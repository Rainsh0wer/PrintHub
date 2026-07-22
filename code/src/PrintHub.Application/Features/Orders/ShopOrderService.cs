using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Notifications;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Application.Specifications.Orders;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders;

/// <summary>
/// Shop-side order operations. Each transition is authorised against the caller's
/// membership of the order's shop (CanOperateShop = owner or active staff) and
/// validated through the <see cref="OrderStateMachine"/> as the shop actor.
/// Declining refunds the customer's wallet in full, mirroring cancellation.
/// </summary>
public class ShopOrderService : IShopOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IProductionQueue _production;
    private readonly INotificationService _notifications;

    public ShopOrderService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper,
        IProductionQueue production, INotificationService notifications)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
        _production = production;
        _notifications = notifications;
    }

    public async Task<Result<PagedResult<OrderSummaryDto>>> ListForShopAsync(
        int shopId, PageRequest page, OrderStatus? status, CancellationToken ct = default)
    {
        if (!_currentUser.CanOperateShop(shopId))
            return Result<PagedResult<OrderSummaryDto>>.Forbidden("You do not have permission to view this shop's orders.");

        var repo = _uow.Repository<Order>();
        var total = await repo.CountAsync(new OrdersByShopCountSpecification(shopId, status), ct);
        var orders = await repo.ListAsync(new OrdersByShopSpecification(shopId, page.Skip, page.Take, status), ct);

        var items = _mapper.Map<IReadOnlyList<OrderSummaryDto>>(orders);
        return Result.Success(new PagedResult<OrderSummaryDto>(items, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<OrderDetailDto>> AcceptAsync(int orderId, CancellationToken ct = default)
    {
        var (order, error) = await LoadOperableAsync(orderId, OrderStatus.Accepted, ct);
        if (error is not null) return error;

        var from = order!.Status;
        order.Status = OrderStatus.Accepted;
        order.AcceptedAt = DateTime.UtcNow;
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, OrderStatus.Accepted, "Order accepted by shop.", ct);
        await _uow.SaveChangesAsync(ct);

        await _notifications.CreateAsync(order.CustomerId, NotificationType.OrderStatus,
            "Order accepted", $"Your order {order.OrderCode} has been accepted by the shop.", order.Id, ct: ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    public async Task<Result<OrderDetailDto>> DeclineAsync(int orderId, DeclineOrderRequest request, CancellationToken ct = default)
    {
        var (order, error) = await LoadOperableAsync(orderId, OrderStatus.Declined, ct);
        if (error is not null) return error;

        var now = DateTime.UtcNow;
        var refund = order!.TotalAmount;

        // A declined order was never produced, so the customer is refunded in full.
        var customer = await _uow.Repository<User>().GetByIdAsync(order.CustomerId, ct);
        if (customer is not null && refund > 0)
        {
            customer.WalletBalance += refund;
            _uow.Repository<User>().Update(customer);
            await _uow.Repository<WalletTransaction>().AddAsync(new WalletTransaction
            {
                UserId = order.CustomerId,
                OrderId = order.Id,
                Type = WalletTransactionType.Refund,
                Amount = refund,
                BalanceAfter = customer.WalletBalance,
                Status = WalletTransactionStatus.Completed,
                RefCode = $"REF-{order.OrderCode}",
                Description = $"Refund for declined order {order.OrderCode}",
                CreatedAt = now
            }, ct);
        }

        var from = order.Status;
        order.Status = OrderStatus.Declined;
        order.DeclineReason = request.Reason;
        order.DeclinedAt = now;
        order.RefundedAmount = refund;
        _uow.Repository<Order>().Update(order);

        var reason = string.IsNullOrWhiteSpace(request.Note) ? request.Reason.ToString() : $"{request.Reason}: {request.Note}";
        await AppendHistoryAsync(order.Id, from, OrderStatus.Declined, reason, ct);
        await _uow.SaveChangesAsync(ct);

        await _notifications.CreateAsync(order.CustomerId, NotificationType.OrderStatus,
            "Order declined", $"Your order {order.OrderCode} was declined and fully refunded.", order.Id, ct: ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    public async Task<Result<OrderDetailDto>> StartProductionAsync(int orderId, StartProductionRequest request, CancellationToken ct = default)
    {
        var (order, error) = await LoadOperableAsync(orderId, OrderStatus.InProduction, ct);
        if (error is not null) return error;

        if (request.MachineId is int machineId)
        {
            var machine = await _uow.Repository<Machine>().GetByIdAsync(machineId, ct);
            if (machine is null || machine.ShopId != order!.ShopId)
                return Result<OrderDetailDto>.Fail("The selected machine does not belong to this shop.");
            if (machine.Status == MachineStatus.Offline)
                return Result<OrderDetailDto>.Conflict("The selected machine is offline.");
            order!.MachineId = machineId;
        }

        var from = order!.Status;
        order.Status = OrderStatus.InProduction;
        order.ProgressPercent = 10;
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, OrderStatus.InProduction, "Production started.", ct);
        await _uow.SaveChangesAsync(ct);

        // Hand the job to the async production pipeline; the agent completes it to
        // ReadyForPickup. Enqueue failures degrade gracefully (order stays InProduction).
        await _production.EnqueueProductionAsync(order.Id, order.OrderCode, ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    public async Task<Result<OrderDetailDto>> MarkReadyAsync(int orderId, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderByIdSpecification(orderId), ct);
        if (order is null) return Result<OrderDetailDto>.NotFound("Order not found.");
        if (!_currentUser.CanOperateShop(order.ShopId))
            return Result<OrderDetailDto>.Forbidden("You do not have permission to operate this order.");

        var target = order.FulfilmentMethod == FulfilmentMethod.Delivery
            ? OrderStatus.OutForDelivery
            : OrderStatus.ReadyForPickup;

        if (!OrderStateMachine.CanTransition(order.Status, target, OrderActor.Shop))
            return Result<OrderDetailDto>.Conflict($"An order in status '{order.Status}' cannot be marked ready.");

        var from = order.Status;
        order.Status = target;
        order.ProgressPercent = 100;
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, target, "Production complete; ready for hand-over.", ct);
        await _uow.SaveChangesAsync(ct);

        await _notifications.CreateAsync(order.CustomerId, NotificationType.OrderStatus,
            "Order ready", $"Your order {order.OrderCode} is ready.", order.Id, ct: ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    public async Task<Result<OrderDetailDto>> HandoverAsync(int orderId, CancellationToken ct = default)
    {
        var (order, error) = await LoadOperableAsync(orderId, OrderStatus.Completed, ct);
        if (error is not null) return error;

        var from = order!.Status;
        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTime.UtcNow;
        order.ProgressPercent = 100;
        order.CommissionRate = OrderService.PlatformCommissionRate;
        order.CommissionAmount = Math.Round(order.TotalAmount * OrderService.PlatformCommissionRate, 2);
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, OrderStatus.Completed, "Handed over to customer.", ct);
        await _uow.SaveChangesAsync(ct);

        await _notifications.CreateAsync(order.CustomerId, NotificationType.OrderStatus,
            "Order completed", $"Your order {order.OrderCode} has been completed.", order.Id, ct: ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    // ---------------------------------------------------------------- helpers

    /// <summary>Loads a tracked order and checks operability + the intended transition in one place.</summary>
    private async Task<(Order? order, Result<OrderDetailDto>? error)> LoadOperableAsync(int orderId, OrderStatus target, CancellationToken ct)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderByIdSpecification(orderId), ct);
        if (order is null)
            return (null, Result<OrderDetailDto>.NotFound("Order not found."));
        if (!_currentUser.CanOperateShop(order.ShopId))
            return (null, Result<OrderDetailDto>.Forbidden("You do not have permission to operate this order."));
        if (!OrderStateMachine.CanTransition(order.Status, target, OrderActor.Shop))
            return (null, Result<OrderDetailDto>.Conflict($"An order in status '{order.Status}' cannot move to '{target}'."));
        return (order, null);
    }

    private async Task AppendHistoryAsync(int orderId, OrderStatus? from, OrderStatus to, string? reason, CancellationToken ct)
        => await _uow.Repository<OrderStatusHistory>().AddAsync(new OrderStatusHistory
        {
            OrderId = orderId,
            FromStatus = from,
            ToStatus = to,
            ActorUserId = _currentUser.UserId,
            ActorRole = _currentUser.Role,
            Reason = reason,
            CreatedAt = DateTime.UtcNow
        }, ct);

    private async Task<OrderDetailDto> LoadDetailAsync(int orderId, CancellationToken ct)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderWithDetailsByIdSpecification(orderId), ct);
        return _mapper.Map<OrderDetailDto>(order!);
    }
}
