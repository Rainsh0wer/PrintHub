using System.Text.Json;
using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Application.Features.Vouchers;
using PrintHub.Application.Specifications.Orders;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders;

/// <summary>
/// Customer-facing order lifecycle. Placement snapshots the chosen quote onto the
/// order and debits the wallet atomically (a single transactional boundary via the
/// unit of work); every status change is appended to the immutable history through
/// the <see cref="OrderStateMachine"/>.
/// </summary>
public class OrderService : IOrderService
{
    /// <summary>Platform commission taken on completion (UC-39 will make this admin-configurable).</summary>
    internal const decimal PlatformCommissionRate = 0.10m;

    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;
    private readonly IVoucherService _vouchers;

    public OrderService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper, IVoucherService vouchers)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
        _vouchers = vouchers;
    }

    public async Task<Result<OrderDetailDto>> PlaceOrderAsync(int customerId, PlaceOrderRequest request, CancellationToken ct = default)
    {
        var quote = await _uow.Repository<Quote>().GetByIdAsync(request.QuoteId, ct);
        if (quote is null || quote.CustomerId != customerId)
            return Result<OrderDetailDto>.NotFound("Quote not found.");
        if (quote.ExpiresAt <= DateTime.UtcNow)
            return Result<OrderDetailDto>.Conflict("This quote has expired. Please compare quotes again.");
        if (quote.IsIndicative)
            return Result<OrderDetailDto>.Conflict("This is an indicative quote; please compare again to get a firm price before ordering.");

        if (request.FulfilmentMethod == FulfilmentMethod.Delivery && string.IsNullOrWhiteSpace(request.DeliveryAddress))
            return Result<OrderDetailDto>.Fail("A delivery address is required for delivery orders.");

        var customer = await _uow.Repository<User>().GetByIdAsync(customerId, ct);
        if (customer is null)
            return Result<OrderDetailDto>.NotFound("Customer not found.");

        // Financials come from the quote, never the client — the quote is authoritative.
        var discount = 0m;
        int? voucherId = null;
        if (!string.IsNullOrWhiteSpace(request.VoucherCode))
        {
            var applied = await _vouchers.ValidateForOrderAsync(customerId, request.VoucherCode, quote.SubTotal, ct);
            if (applied.IsFailure)
                return Result<OrderDetailDto>.Fail(applied.Error!, applied.ErrorType);
            discount = applied.Value!.Discount;
            voucherId = applied.Value.VoucherId;
        }

        var total = quote.SubTotal - discount;
        if (customer.WalletBalance < total)
            return Result<OrderDetailDto>.Conflict("Insufficient wallet balance. Please top up your wallet and try again.");

        var lines = DeserializeBreakdown(quote.BreakdownJson);
        var now = DateTime.UtcNow;

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var order = new Order
            {
                OrderCode = "PENDING",            // replaced with the id-derived code after the first save
                CustomerId = customerId,
                ShopId = quote.ShopId,
                QuoteId = quote.Id,
                VoucherId = voucherId,
                Status = OrderStatus.AwaitingAcceptance,
                FulfilmentMethod = request.FulfilmentMethod,
                PickupSlotStart = request.PickupSlotStart,
                PickupSlotEnd = request.PickupSlotEnd,
                DeliveryAddress = request.DeliveryAddress,
                SubTotal = quote.SubTotal,
                DiscountAmount = discount,
                TotalAmount = total,
                ProgressPercent = 0,
                CustomerNote = request.CustomerNote,
                PlacedAt = now,
                EstimatedReadyAt = now.AddMinutes(quote.EstimatedMinutes),
                Items = BuildItems(request.Items, lines)
            };
            await _uow.Repository<Order>().AddAsync(order, ct);
            await _uow.SaveChangesAsync(ct);      // assigns order + item ids

            order.OrderCode = $"PH-{now:yyMMdd}-{order.Id:D4}";
            _uow.Repository<Order>().Update(order);

            if (voucherId is int vid)
            {
                var voucher = await _uow.Repository<Voucher>().GetByIdAsync(vid, ct);
                if (voucher is not null)
                {
                    voucher.UsedCount++;
                    _uow.Repository<Voucher>().Update(voucher);
                }
            }

            // Debit the wallet and record the ledger entry against the resulting balance.
            customer.WalletBalance -= total;
            _uow.Repository<User>().Update(customer);
            await _uow.Repository<WalletTransaction>().AddAsync(new WalletTransaction
            {
                UserId = customerId,
                OrderId = order.Id,
                Type = WalletTransactionType.Payment,
                Amount = -total,
                BalanceAfter = customer.WalletBalance,
                Status = WalletTransactionStatus.Completed,
                RefCode = $"PAY-{order.OrderCode}",
                Description = $"Payment for order {order.OrderCode}",
                CreatedAt = now
            }, ct);

            await AppendHistoryAsync(order.Id, null, OrderStatus.AwaitingAcceptance, "Order placed.", ct);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return Result.Success(await LoadDetailAsync(order.Id, ct));
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<Result<PagedResult<OrderSummaryDto>>> ListForCustomerAsync(
        int customerId, PageRequest page, OrderStatus? status, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Order>();
        var total = await repo.CountAsync(new OrdersByCustomerCountSpecification(customerId, status), ct);
        var orders = await repo.ListAsync(new OrdersByCustomerSpecification(customerId, page.Skip, page.Take, status), ct);

        var items = _mapper.Map<IReadOnlyList<OrderSummaryDto>>(orders);
        return Result.Success(new PagedResult<OrderSummaryDto>(items, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<OrderDetailDto>> GetByIdAsync(int orderId, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderWithDetailsByIdSpecification(orderId), ct);
        if (order is null)
            return Result<OrderDetailDto>.NotFound("Order not found.");

        var isCustomer = _currentUser.UserId == order.CustomerId;
        var isShop = _currentUser.CanOperateShop(order.ShopId);
        if (!isCustomer && !isShop)
            return Result<OrderDetailDto>.Forbidden("You do not have permission to view this order.");

        return Result.Success(_mapper.Map<OrderDetailDto>(order));
    }

    public async Task<Result<OrderDetailDto>> CancelAsync(int customerId, int orderId, CancelOrderRequest request, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderByIdSpecification(orderId), ct);
        if (order is null || order.CustomerId != customerId)
            return Result<OrderDetailDto>.NotFound("Order not found.");

        if (!OrderStateMachine.CanTransition(order.Status, OrderStatus.Cancelled, OrderActor.Customer))
            return Result<OrderDetailDto>.Conflict($"An order in status '{order.Status}' can no longer be cancelled.");

        // Cancellation is only permitted before production starts, so a full refund applies.
        var refund = order.TotalAmount;
        var now = DateTime.UtcNow;

        var customer = await _uow.Repository<User>().GetByIdAsync(customerId, ct);
        if (customer is not null && refund > 0)
        {
            customer.WalletBalance += refund;
            _uow.Repository<User>().Update(customer);
            await _uow.Repository<WalletTransaction>().AddAsync(new WalletTransaction
            {
                UserId = customerId,
                OrderId = order.Id,
                Type = WalletTransactionType.Refund,
                Amount = refund,
                BalanceAfter = customer.WalletBalance,
                Status = WalletTransactionStatus.Completed,
                RefCode = $"REF-{order.OrderCode}",
                Description = $"Refund for cancelled order {order.OrderCode}",
                CreatedAt = now
            }, ct);
        }

        var from = order.Status;
        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = now;
        order.CancellationReason = request.Reason;
        order.RefundedAmount = refund;
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, OrderStatus.Cancelled, request.Reason ?? "Cancelled by customer.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    public async Task<Result<OrderDetailDto>> ConfirmReceiptAsync(int customerId, int orderId, CancellationToken ct = default)
    {
        var order = await _uow.Repository<Order>().FirstOrDefaultAsync(new OrderByIdSpecification(orderId), ct);
        if (order is null || order.CustomerId != customerId)
            return Result<OrderDetailDto>.NotFound("Order not found.");

        if (!OrderStateMachine.CanTransition(order.Status, OrderStatus.Completed, OrderActor.Customer))
            return Result<OrderDetailDto>.Conflict($"An order in status '{order.Status}' cannot be confirmed as received.");

        var from = order.Status;
        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTime.UtcNow;
        order.ProgressPercent = 100;
        order.CommissionRate = PlatformCommissionRate;
        order.CommissionAmount = Math.Round(order.TotalAmount * PlatformCommissionRate, 2);
        _uow.Repository<Order>().Update(order);

        await AppendHistoryAsync(order.Id, from, OrderStatus.Completed, "Receipt confirmed by customer.", ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(await LoadDetailAsync(order.Id, ct));
    }

    // ---------------------------------------------------------------- helpers

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

    private static List<OrderItem> BuildItems(IReadOnlyList<PlaceOrderItemInput> inputs, IReadOnlyList<QuoteLine> lines)
    {
        var items = new List<OrderItem>();
        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var line = i < lines.Count ? lines[i] : null;
            items.Add(new OrderItem
            {
                ServiceTypeId = input.ServiceTypeId,
                DocumentFileId = input.DocumentFileId,
                Quantity = input.Quantity,
                PageCount = input.PageCount,
                PaperType = input.PaperType,
                ColorMode = input.ColorMode,
                Sides = input.Sides,
                BindingType = input.BindingType,
                MaterialName = input.MaterialName,
                QualityProfile = input.QualityProfile,
                EstimatedGrams = input.EstimatedGrams,
                UnitPrice = line?.EffectiveUnitPrice ?? 0m,
                LineTotal = line?.LineTotal ?? 0m,
                EstimatedMinutes = line?.Minutes,
                ItemNote = input.ItemNote
            });
        }
        return items;
    }

    private static IReadOnlyList<QuoteLine> DeserializeBreakdown(string? breakdownJson)
    {
        if (string.IsNullOrWhiteSpace(breakdownJson)) return Array.Empty<QuoteLine>();
        try
        {
            return JsonSerializer.Deserialize<List<QuoteLine>>(breakdownJson) ?? new List<QuoteLine>();
        }
        catch (JsonException)
        {
            return Array.Empty<QuoteLine>();
        }
    }
}
