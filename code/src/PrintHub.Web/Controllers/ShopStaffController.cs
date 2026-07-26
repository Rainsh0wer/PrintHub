using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopStaffController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopStaffController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopStaff" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new List<StaffDto>()); }
        var res = await _api.GetAsync<List<StaffDto>>($"/api/shops/{shopId}/staff");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<StaffDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Grant(string email, string? position)
    {
        var shopId = CurrentShopId();
        var res = await _api.PostAsync<StaffDto>($"/api/shops/{shopId}/staff", new { email, position });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Staff access granted." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Revoke(int staffId)
    {
        var shopId = CurrentShopId();
        var res = await _api.DeleteAsync($"/api/shops/{shopId}/staff/{staffId}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Staff access revoked." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
