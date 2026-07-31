using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminController(PrintHubApiClient api) => _api = api;

    public IActionResult Index() => RedirectToAction(nameof(Approvals));

    public async Task<IActionResult> Approvals()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/Admin/Approvals" });
        var pending = await _api.GetAsync<List<ShopAdminListItemDto>>("/api/admin/shops/applications");
        var active = await _api.GetAsync<PagedResult<ShopSummaryDto>>("/api/shops?PageSize=50");
        // Suspended shops are excluded from public search (BR-21), so they must be
        // fetched separately or an administrator could never reinstate them.
        var suspended = await _api.GetAsync<List<ShopAdminListItemDto>>("/api/admin/shops?status=4");

        ViewBag.Active = active.Data?.Items ?? new List<ShopSummaryDto>();
        ViewBag.Suspended = suspended.Data ?? new List<ShopAdminListItemDto>();
        return View(pending.Data ?? new List<ShopAdminListItemDto>());
    }

    [HttpPost] public Task<IActionResult> Approve(int id) => Act($"/api/admin/shops/{id}/approve", null, "Shop approved.");
    [HttpPost] public Task<IActionResult> Reject(int id, string reason) => Act($"/api/admin/shops/{id}/reject", new { reason }, "Application rejected.");
    [HttpPost] public Task<IActionResult> Suspend(int id, string reason) => Act($"/api/admin/shops/{id}/suspend", new { reason }, "Shop suspended.");
    [HttpPost] public Task<IActionResult> Reinstate(int id) => Act($"/api/admin/shops/{id}/reinstate", null, "Shop reinstated — it can receive orders again.");

    private async Task<IActionResult> Act(string path, object? body, string ok)
    {
        var res = await _api.PutAsync<object>(path, body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? ok : res.Error;
        return RedirectToAction(nameof(Approvals));
    }
}
