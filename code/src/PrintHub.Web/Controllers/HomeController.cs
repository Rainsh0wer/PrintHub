using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Models;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class HomeController : Controller
{
    private readonly PrintHubApiClient _api;

    public HomeController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        var res = await _api.GetAsync<PagedResult<ShopSummaryDto>>("/api/shops?PageSize=8");
        return View(res.Data ?? new PagedResult<ShopSummaryDto>());
    }

    public IActionResult Privacy() => View();

    public IActionResult Status(int id)
    {
        ViewBag.Code = id;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
