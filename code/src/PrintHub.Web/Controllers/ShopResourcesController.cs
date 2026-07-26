using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopResourcesController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopResourcesController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Machines()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopResources/Machines" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new List<MachineAdminDto>()); }
        var res = await _api.GetAsync<List<MachineAdminDto>>($"/api/shops/{shopId}/machines");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<MachineAdminDto>());
    }

    [HttpPost]
    public async Task<IActionResult> SetStatus(int machineId, int status)
    {
        var shopId = CurrentShopId();
        var res = await _api.PutAsync<MachineAdminDto>($"/api/shops/{shopId}/machines/{machineId}/status", new { status });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Machine updated." : res.Error;
        return RedirectToAction(nameof(Machines));
    }

    public async Task<IActionResult> Materials()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopResources/Materials" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new List<MaterialAdminDto>()); }
        var res = await _api.GetAsync<List<MaterialAdminDto>>($"/api/shops/{shopId}/materials");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<MaterialAdminDto>());
    }

    [HttpPost]
    public async Task<IActionResult> AdjustStock(int materialId, decimal stockQuantity, decimal lowStockThreshold)
    {
        var shopId = CurrentShopId();
        var res = await _api.PutAsync<MaterialAdminDto>($"/api/shops/{shopId}/materials/{materialId}/stock", new { stockQuantity, lowStockThreshold });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Stock updated." : res.Error;
        return RedirectToAction(nameof(Materials));
    }
}
