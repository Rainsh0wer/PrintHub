using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders;

/// <summary>
/// Customer-facing order use cases: placement (wallet payment + quote snapshot),
/// tracking, history, cancellation (rule-based refund) and receipt confirmation.
/// Shop-side operations live in <see cref="IShopOrderService"/>.
/// </summary>
public interface IOrderService
{
    /// <summary>UC-15 — place an order from a quote, paying from the wallet, atomically.</summary>
    Task<Result<OrderDetailDto>> PlaceOrderAsync(int customerId, PlaceOrderRequest request, CancellationToken ct = default);

    /// <summary>UC-20 — the caller's order history, newest first, paged and optionally filtered.</summary>
    Task<Result<PagedResult<OrderSummaryDto>>> ListForCustomerAsync(int customerId, PageRequest page, OrderStatus? status, CancellationToken ct = default);

    /// <summary>UC-16 — an order's full detail and history (customer or fulfilling shop only).</summary>
    Task<Result<OrderDetailDto>> GetByIdAsync(int orderId, CancellationToken ct = default);

    /// <summary>UC-17 — cancel before production; refunds the wallet per the cancellation rule.</summary>
    Task<Result<OrderDetailDto>> CancelAsync(int customerId, int orderId, CancelOrderRequest request, CancellationToken ct = default);

    /// <summary>UC-19 — confirm collection/receipt; completes the order and records commission.</summary>
    Task<Result<OrderDetailDto>> ConfirmReceiptAsync(int customerId, int orderId, CancellationToken ct = default);
}
