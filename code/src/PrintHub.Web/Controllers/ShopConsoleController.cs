using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopConsoleController : Controller
{
    private readonly PrintHubApiClient _api;

    public ShopConsoleController(PrintHubApiClient api) => _api = api;

    public IActionResult Index() => RedirectToAction(nameof(Orders));

    public async Task<IActionResult> Orders(string? status)
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopConsole/Orders" });
        var shopId = ShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new PagedResult<OrderSummaryDto>()); }

        var path = $"/api/shops/{shopId}/orders?PageSize=50" + (string.IsNullOrEmpty(status) ? "" : $"&status={status}");
        var res = await _api.GetAsync<PagedResult<OrderSummaryDto>>(path);
        ViewBag.Error = res.Ok ? null : res.Error;
        ViewBag.Status = status;
        return View(res.Data ?? new PagedResult<OrderSummaryDto>());
    }

    [HttpPost] public Task<IActionResult> Accept(int id) => Act(id, $"/api/orders/{id}/accept", null, "Order accepted.");
    [HttpPost] public Task<IActionResult> Decline(int id, int reason, string? note) => Act(id, $"/api/orders/{id}/decline", new { reason, note }, "Order declined and refunded.");
    [HttpPost] public Task<IActionResult> Start(int id, int? machineId) => Act(id, $"/api/orders/{id}/start", new { machineId }, "Production started.");
    [HttpPost] public Task<IActionResult> Ready(int id) => Act(id, $"/api/orders/{id}/ready", null, "Marked ready.");
    [HttpPost] public Task<IActionResult> Handover(int id) => Act(id, $"/api/orders/{id}/handover", null, "Handed over.");

    private async Task<IActionResult> Act(int id, string path, object? body, string ok)
    {
        var res = await _api.PutAsync<OrderDetailDto>(path, body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? ok : res.Error;
        return RedirectToAction(nameof(Orders));
    }

    private bool IsShop() => HttpContext.Session.GetString(SessionKeys.UserRole) is "ShopOwner" or "ShopStaff";

    private int? ShopId()
    {
        var csv = HttpContext.Session.GetString(SessionKeys.ShopIds);
        var first = csv?.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return int.TryParse(first, out var v) ? v : null;
    }
}
