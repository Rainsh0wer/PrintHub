using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Application.Features.Platform.Dtos;
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

        // UC-39 also covers the platform commission rate, configured on this screen.
        var commission = await _api.GetAsync<CommissionDto>("/api/admin/commission");
        ViewBag.CommissionRate = commission.Data?.CommissionRate ?? 0.10m;
        ViewBag.CommissionUpdatedAt = commission.Data?.UpdatedAt;

        var cancellation = await _api.GetAsync<CancellationFeeDto>("/api/admin/cancellation-fee");
        ViewBag.CancellationFeeRate = cancellation.Data?.CancellationFeeRate ?? 0.10m;

        return View(res.Data ?? new List<ServiceTypeAdminDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Create(string code, string name, int serviceGroup, int pricingModel,
        string unitOfMeasure, bool requiresFile, int displayOrder)
    {
        var body = new
        {
            code, name, serviceGroup, pricingModel, unitOfMeasure,
            requiresFile, description = (string?)null, displayOrder, iconUrl = (string?)null
        };
        var res = await _api.PostAsync<ServiceTypeAdminDto>("/api/admin/service-types", body);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service type created." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>UC-39 — edit an existing service type. Code and pricing model are fixed
    /// once orders exist (BR-106), so only the editable fields are sent.</summary>
    [HttpPost]
    public async Task<IActionResult> Update(int id, string name, string unitOfMeasure,
        bool requiresFile, bool isActive, int displayOrder, string? description)
    {
        var res = await _api.PutAsync<ServiceTypeAdminDto>($"/api/admin/service-types/{id}",
            new { name, unitOfMeasure, requiresFile, description, isActive, displayOrder, iconUrl = (string?)null });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service type updated." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var res = await _api.DeleteAsync($"/api/admin/service-types/{id}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service type deactivated." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>UC-39 — the platform commission applied to orders completed from now on.</summary>
    [HttpPost]
    public async Task<IActionResult> SetCommission(decimal ratePercent)
    {
        var res = await _api.PutAsync<CommissionDto>("/api/admin/commission", new { rate = ratePercent / 100m });
        TempData[res.Ok ? "ok" : "err"] = res.Ok
            ? $"Commission set to {ratePercent:0.##}%. Orders completed from now on use the new rate."
            : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>BR-47 — what a shop keeps when a customer cancels an order it had accepted.</summary>
    [HttpPost]
    public async Task<IActionResult> SetCancellationFee(decimal ratePercent)
    {
        var res = await _api.PutAsync<CancellationFeeDto>("/api/admin/cancellation-fee", new { rate = ratePercent / 100m });
        TempData[res.Ok ? "ok" : "err"] = res.Ok
            ? $"Cancellation fee set to {ratePercent:0.##}% of the order total."
            : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
