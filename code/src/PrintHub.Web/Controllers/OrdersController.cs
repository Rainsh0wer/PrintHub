using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Enums;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class OrdersController : Controller
{
    private readonly PrintHubApiClient _api;

    public OrdersController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (NotCustomer()) return RedirectToAction("Login", "Account", new { returnUrl = "/Orders" });
        var res = await _api.GetAsync<PagedResult<OrderSummaryDto>>("/api/orders?PageSize=30");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<OrderSummaryDto>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var res = await _api.GetAsync<OrderDetailDto>($"/api/orders/{id}");
        if (!res.Ok || res.Data is null)
        {
            ViewBag.Error = res.Error ?? "Order not found.";
            return View((OrderDetailDto?)null);
        }
        return View(res.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Place(int quoteId, int serviceTypeId, int quantity,
        int? pageCount, int colorMode, int sides, decimal? estimatedGrams, string? voucherCode)
    {
        var item = new PlaceOrderItemInput(serviceTypeId, null, Math.Max(1, quantity), pageCount,
            null, (ColorMode)colorMode, (Sides)sides, null, null, null, estimatedGrams, null);
        var request = new PlaceOrderRequest(quoteId, FulfilmentMethod.Pickup, null, null, null, null,
            new List<PlaceOrderItemInput> { item }, string.IsNullOrWhiteSpace(voucherCode) ? null : voucherCode);

        var res = await _api.PostAsync<OrderDetailDto>("/api/orders", request);
        if (res.Ok && res.Data is not null)
            return RedirectToAction(nameof(Details), new { id = res.Data.Id });

        TempData["err"] = res.Error ?? "Could not place the order.";
        return RedirectToAction("Index", "Quotes");
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id, string? reason)
    {
        var res = await _api.PutAsync<OrderDetailDto>($"/api/orders/{id}/cancel", new { reason });
        if (!res.Ok) TempData["err"] = res.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Confirm(int id)
    {
        var res = await _api.PutAsync<OrderDetailDto>($"/api/orders/{id}/confirm-receipt", null);
        if (!res.Ok) TempData["err"] = res.Error;
        return RedirectToAction(nameof(Details), new { id });
    }

    private bool NotCustomer() => HttpContext.Session.GetString(SessionKeys.UserRole) != "Customer";
}
