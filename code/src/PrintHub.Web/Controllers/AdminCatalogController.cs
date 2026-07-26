using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminCatalogController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminCatalogController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminCatalog" });
        var res = await _api.GetAsync<List<ServiceTypeAdminDto>>("/api/admin/service-types");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<ServiceTypeAdminDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string code, string name, int serviceGroup, int pricingModel, string unitOfMeasure)
    {
        var body = new { code, name, serviceGroup, pricingModel, unitOfMeasure, requiresFile = true, displayOrder = 50 };
        var res = await _api.PostAsync<ServiceTypeAdminDto>("/api/admin/service-types", body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service type created." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var res = await _api.DeleteAsync($"/api/admin/service-types/{id}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service type deactivated." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
