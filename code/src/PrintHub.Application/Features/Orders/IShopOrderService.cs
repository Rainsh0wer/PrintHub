using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders;

/// <summary>
/// Shop-facing order operations (UC-31..34). Every method is scoped to the
/// fulfilling shop: the caller must own or be active staff of the order's shop,
/// otherwise the operation is forbidden. Transitions go through the
/// <see cref="OrderStateMachine"/> as the shop actor.
/// </summary>
public interface IShopOrderService
{
    /// <summary>UC-31 — the shop's order queue, newest first, paged and optionally filtered.</summary>
    Task<Result<PagedResult<OrderSummaryDto>>> ListForShopAsync(int shopId, PageRequest page, OrderStatus? status, CancellationToken ct = default);

    /// <summary>UC-32 — accept an incoming order (AwaitingAcceptance → Accepted).</summary>
    Task<Result<OrderDetailDto>> AcceptAsync(int orderId, CancellationToken ct = default);

    /// <summary>UC-32 — decline with a reason (AwaitingAcceptance → Declined); refunds the customer in full.</summary>
    Task<Result<OrderDetailDto>> DeclineAsync(int orderId, DeclineOrderRequest request, CancellationToken ct = default);

    /// <summary>UC-33 — start production, assigning a machine (Accepted → InProduction).</summary>
    Task<Result<OrderDetailDto>> StartProductionAsync(int orderId, StartProductionRequest request, CancellationToken ct = default);

    /// <summary>Production complete (InProduction → ReadyForPickup / OutForDelivery by fulfilment method).</summary>
    Task<Result<OrderDetailDto>> MarkReadyAsync(int orderId, CancellationToken ct = default);

    /// <summary>UC-34 — record hand-over to the customer (ReadyForPickup / OutForDelivery → Completed).</summary>
    Task<Result<OrderDetailDto>> HandoverAsync(int orderId, CancellationToken ct = default);
}
