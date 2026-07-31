using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

/// <summary>UC-11 — the customer's saved shops.</summary>
public class FavouritesController : Controller
{
    private readonly PrintHubApiClient _api;
    public FavouritesController(PrintHubApiClient api) => _api = api;

    private bool NotCustomer() => HttpContext.Session.GetString(SessionKeys.UserRole) != "Customer";

    public async Task<IActionResult> Index()
    {
        if (NotCustomer()) return RedirectToAction("Login", "Account", new { returnUrl = "/Favourites" });
        var res = await _api.GetAsync<List<ShopSummaryDto>>("/api/favourites");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<ShopSummaryDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Add(int shopId, string? returnUrl)
    {
        var res = await _api.PostAsync<object>($"/api/favourites/{shopId}", null);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Saved to your favourites." : res.Error;
        return SafeRedirect(returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int shopId, string? returnUrl)
    {
        var res = await _api.DeleteAsync($"/api/favourites/{shopId}");
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "Removed from your favourites." : res.Error;
        return SafeRedirect(returnUrl);
    }

    /// <summary>
    /// returnUrl comes from the request, so only a local path is honoured — a full URL
    /// would otherwise turn this into an open redirect to an attacker's site.
    /// </summary>
    private IActionResult SafeRedirect(string? returnUrl)
        => !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(Index));
}
