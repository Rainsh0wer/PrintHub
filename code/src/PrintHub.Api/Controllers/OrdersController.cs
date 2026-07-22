using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Customer order lifecycle (UC-15 place, UC-16 track, UC-17 cancel, UC-19 confirm,
/// UC-20 history). Ownership is enforced in the service against the authenticated
/// caller, so a crafted order id returns 404/403 rather than another user's order.
/// </summary>
[ApiController]
[Route("api/orders")]
[Authorize]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    private readonly ICurrentUser _currentUser;

    public OrdersController(IOrderService orders, ICurrentUser currentUser)
    {
        _orders = orders;
        _currentUser = currentUser;
    }

    /// <summary>UC-15 — place an order from a quote, paid from the wallet.</summary>
    [HttpPost]
    public async Task<IActionResult> Place(PlaceOrderRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _orders.PlaceOrderAsync(userId.Value, request, ct))
            .ToActionResult(StatusCodes.Status201Created, "Order placed.");
    }

    /// <summary>UC-20 — the caller's order history (paged, optional status filter).</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PageRequest page, [FromQuery] OrderStatus? status, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _orders.ListForCustomerAsync(userId.Value, page, status, ct)).ToActionResult();
    }

    /// <summary>UC-16 — track an order: status, progress, and full history.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => (await _orders.GetByIdAsync(id, ct)).ToActionResult();

    /// <summary>UC-17 — cancel before production (rule-based refund to the wallet).</summary>
    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancelOrderRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _orders.CancelAsync(userId.Value, id, request, ct)).ToActionResult(successMessage: "Order cancelled.");
    }

    /// <summary>UC-19 — confirm collection/receipt; completes the order.</summary>
    [HttpPut("{id:int}/confirm-receipt")]
    public async Task<IActionResult> ConfirmReceipt(int id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _orders.ConfirmReceiptAsync(userId.Value, id, ct)).ToActionResult(successMessage: "Order completed.");
    }
}
