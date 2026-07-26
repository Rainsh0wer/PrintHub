using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
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
        if (shopId is null) { ViewBag.NoShop = true; return View(new List<RateCardEntryDto>()); }

        var res = await _api.GetAsync<List<RateCardEntryDto>>($"/api/shops/{shopId}/rate-card");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<RateCardEntryDto>());
    }
}
