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
        ViewBag.Active = active.Data?.Items ?? new List<ShopSummaryDto>();
        return View(pending.Data ?? new List<ShopAdminListItemDto>());
    }

    [HttpPost] public Task<IActionResult> Approve(int id) => Act($"/api/admin/shops/{id}/approve", null, "Shop approved.");
    [HttpPost] public Task<IActionResult> Reject(int id, string reason) => Act($"/api/admin/shops/{id}/reject", new { reason }, "Application rejected.");
    [HttpPost] public Task<IActionResult> Suspend(int id, string reason) => Act($"/api/admin/shops/{id}/suspend", new { reason }, "Shop suspended.");

    private async Task<IActionResult> Act(string path, object? body, string ok)
    {
        var res = await _api.PutAsync<object>(path, body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? ok : res.Error;
        return RedirectToAction(nameof(Approvals));
    }
}
