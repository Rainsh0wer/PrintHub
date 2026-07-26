using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Reports.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopReportsController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public ShopReportsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsShop()) return RedirectToAction("Login", "Account", new { returnUrl = "/ShopReports" });
        var shopId = CurrentShopId();
        if (shopId is null) { ViewBag.NoShop = true; return View(new ShopRevenueReportDto()); }
        var res = await _api.GetAsync<ShopRevenueReportDto>($"/api/shops/{shopId}/reports/revenue");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new ShopRevenueReportDto());
    }
}
