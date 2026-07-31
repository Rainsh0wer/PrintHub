using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

/// <summary>UC-26 — the shop owner's storefront profile.</summary>
public class ShopProfileController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopProfileController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopProfile" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View((ShopDetailDto?)null); }

        var res = await _api.GetAsync<ShopDetailDto>($"/api/shops/{shopId}");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string name, string? description, string addressLine,
        string district, string city, string? phoneNumber, string openTime, string closeTime)
    {
        var shopId = CurrentShopId();
        var res = await _api.PutAsync<object>($"/api/shops/{shopId}/profile", new
        {
            name,
            description,
            addressLine,
            district,
            city,
            phoneNumber,
            openTime = NormaliseTime(openTime),
            closeTime = NormaliseTime(closeTime)
        });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Storefront updated." : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>An &lt;input type="time"&gt; posts "HH:mm"; the API expects "HH:mm:ss".</summary>
    private static string NormaliseTime(string? value)
        => string.IsNullOrWhiteSpace(value) ? "08:00:00" : (value.Count(c => c == ':') == 1 ? value + ":00" : value);
}
