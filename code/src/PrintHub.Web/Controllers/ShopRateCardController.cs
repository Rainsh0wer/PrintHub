using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Models;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopRateCardController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopRateCardController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopRateCard" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new RateCardVm()); }

        var res = await _api.GetAsync<List<RateCardEntryDto>>($"/api/shops/{shopId}/rate-card");
        ViewBag.Error = res.Ok ? null : res.Error;

        var entries = res.Data ?? new List<RateCardEntryDto>();
        var catalogue = await _api.GetAsync<List<ServiceTypeAdminDto>>("/api/service-types");

        // Only offer service types the shop has not priced yet — one entry per type (BR-27/409).
        var used = entries.Select(e => e.ServiceTypeId).ToHashSet();
        return View(new RateCardVm
        {
            Entries = entries,
            AvailableServices = (catalogue.Data ?? new()).Where(s => !used.Contains(s.Id)).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddEntry(int serviceTypeId, decimal unitPrice, decimal setupFee,
        int minQuantity, int leadTimeMinutes)
    {
        var shopId = CurrentShopId();
        var res = await _api.PostAsync<RateCardEntryDto>($"/api/shops/{shopId}/rate-card",
            new AddRateCardEntryRequest(serviceTypeId, unitPrice, setupFee, Math.Max(1, minQuantity), leadTimeMinutes));
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Service added to the rate card." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEntry(int entryId, decimal unitPrice, decimal setupFee,
        int minQuantity, int leadTimeMinutes, bool isActive)
    {
        var shopId = CurrentShopId();
        var res = await _api.PutAsync<RateCardEntryDto>($"/api/shops/{shopId}/rate-card/{entryId}",
            new UpdateRateCardEntryRequest(unitPrice, setupFee, Math.Max(1, minQuantity), leadTimeMinutes, isActive));
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Pricing updated." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>BR-73 — a priced service is deactivated, never hard-deleted, so past orders stay intact.</summary>
    [HttpPost]
    public async Task<IActionResult> ToggleEntry(int entryId, decimal unitPrice, decimal setupFee,
        int minQuantity, int leadTimeMinutes, bool isActive)
    {
        var shopId = CurrentShopId();
        var res = await _api.PutAsync<RateCardEntryDto>($"/api/shops/{shopId}/rate-card/{entryId}",
            new UpdateRateCardEntryRequest(unitPrice, setupFee, Math.Max(1, minQuantity), leadTimeMinutes, isActive));
        TempData[res.Ok ? "ok" : "err"] = res.Ok
            ? (isActive ? "Service re-enabled — it can be quoted again." : "Service disabled — hidden from search and quoting. Existing orders are unaffected.")
            : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AddRule(int entryId, int ruleType, string optionKey,
        decimal multiplier, decimal flatExtra, int? minQuantity, int? maxQuantity)
    {
        var shopId = CurrentShopId();
        var res = await _api.PostAsync<object>($"/api/shops/{shopId}/rate-card/{entryId}/rules",
            new
            {
                ruleType,
                optionKey = optionKey?.Trim(),
                multiplier,
                flatExtra,
                minQuantity,
                maxQuantity
            });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Pricing rule added." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveRule(int entryId, int ruleId)
    {
        var shopId = CurrentShopId();
        var res = await _api.DeleteAsync($"/api/shops/{shopId}/rate-card/{entryId}/rules/{ruleId}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Pricing rule removed." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
