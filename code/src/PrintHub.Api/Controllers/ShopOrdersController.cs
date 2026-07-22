using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Shop-side order operations (UC-31 queue, UC-32 accept/decline, UC-33 start,
/// UC-34 hand-over). Authorization is scoped inside the service: the caller must
/// own or be active staff of the order's shop, so acting on another shop's order
/// returns 403 rather than mutating it.
/// </summary>
[ApiController]
[Route("api")]
[Authorize]
[Produces("application/json")]
public class ShopOrdersController : ControllerBase
{
    private readonly IShopOrderService _orders;

    public ShopOrdersController(IShopOrderService orders) => _orders = orders;

    /// <summary>UC-31 — a shop's order queue (filter by status).</summary>
    [HttpGet("shops/{shopId:int}/orders")]
    public async Task<IActionResult> Queue(int shopId, [FromQuery] PageRequest page, [FromQuery] OrderStatus? status, CancellationToken ct)
        => (await _orders.ListForShopAsync(shopId, page, status, ct)).ToActionResult();

    /// <summary>UC-32 — accept an incoming order.</summary>
    [HttpPut("orders/{id:int}/accept")]
    public async Task<IActionResult> Accept(int id, CancellationToken ct)
        => (await _orders.AcceptAsync(id, ct)).ToActionResult(successMessage: "Order accepted.");

    /// <summary>UC-32 — decline an order with a reason (auto full refund).</summary>
    [HttpPut("orders/{id:int}/decline")]
    public async Task<IActionResult> Decline(int id, DeclineOrderRequest request, CancellationToken ct)
        => (await _orders.DeclineAsync(id, request, ct)).ToActionResult(successMessage: "Order declined and refunded.");

    /// <summary>UC-33 — assign a machine and start production.</summary>
    [HttpPut("orders/{id:int}/start")]
    public async Task<IActionResult> Start(int id, StartProductionRequest request, CancellationToken ct)
        => (await _orders.StartProductionAsync(id, request, ct)).ToActionResult(successMessage: "Production started.");

    /// <summary>Mark production complete (ready for pickup / out for delivery).</summary>
    [HttpPut("orders/{id:int}/ready")]
    public async Task<IActionResult> Ready(int id, CancellationToken ct)
        => (await _orders.MarkReadyAsync(id, ct)).ToActionResult(successMessage: "Order ready.");

    /// <summary>UC-34 — record hand-over to the customer (completes the order).</summary>
    [HttpPut("orders/{id:int}/handover")]
    public async Task<IActionResult> Handover(int id, CancellationToken ct)
        => (await _orders.HandoverAsync(id, ct)).ToActionResult(successMessage: "Order handed over.");
}
