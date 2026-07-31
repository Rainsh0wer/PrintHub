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

    [HttpPost]
    public async Task<IActionResult> AddMachine(string name, int machineType, int serviceGroup)
    {
        var shopId = CurrentShopId();
        var res = await _api.PostAsync<MachineAdminDto>($"/api/shops/{shopId}/machines",
            new { name, machineType, serviceGroup });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Machine added." : res.Error;
        return RedirectToAction(nameof(Machines));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMachine(int machineId)
    {
        var shopId = CurrentShopId();
        var res = await _api.DeleteAsync($"/api/shops/{shopId}/machines/{machineId}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Machine removed." : res.Error;
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

    [HttpPost]
    public async Task<IActionResult> AddMaterial(string name, int materialType, string unit,
        decimal stockQuantity, decimal lowStockThreshold, decimal unitCost)
    {
        var shopId = CurrentShopId();
        var res = await _api.PostAsync<MaterialAdminDto>($"/api/shops/{shopId}/materials",
            new { name, materialType, unit, stockQuantity, lowStockThreshold, unitCost });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Material added." : res.Error;
        return RedirectToAction(nameof(Materials));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveMaterial(int materialId)
    {
        var shopId = CurrentShopId();
        var res = await _api.DeleteAsync($"/api/shops/{shopId}/materials/{materialId}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Material removed." : res.Error;
        return RedirectToAction(nameof(Materials));
    }
}
