using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ShopsController : Controller
{
    private readonly PrintHubApiClient _api;

    public ShopsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? q, string? group)
    {
        var path = $"/api/shops?PageSize=24"
            + (string.IsNullOrWhiteSpace(q) ? "" : $"&Keyword={Uri.EscapeDataString(q)}")
            + (string.IsNullOrWhiteSpace(group) ? "" : $"&ServiceGroup={Uri.EscapeDataString(group)}");
        var res = await _api.GetAsync<PagedResult<ShopSummaryDto>>(path);
        ViewBag.Query = q;
        ViewBag.Group = group;
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<ShopSummaryDto>());
    }

    public async Task<IActionResult> Details(int id)
    {
        var res = await _api.GetAsync<ShopDetailDto>($"/api/shops/{id}");
        if (!res.Ok || res.Data is null)
        {
            ViewBag.Error = res.Error ?? "Shop not found.";
            return View((ShopDetailDto?)null);
        }
        return View(res.Data);
    }
}
