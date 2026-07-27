using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

/// <summary>"Become a seller" (UC-25): apply to open a shop and track the application status.</summary>
public class SellController : Controller
{
    private readonly PrintHubApiClient _api;
    public SellController(PrintHubApiClient api) => _api = api;

    private bool IsSignedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeys.AccessToken));

    /// <summary>Landing page: pitch for guests, application status + form for signed-in users.</summary>
    public async Task<IActionResult> Index()
    {
        if (!IsSignedIn()) { ViewBag.Anonymous = true; return View(new List<ShopApplicationDto>()); }

        var res = await _api.GetAsync<List<ShopApplicationDto>>("/api/shops/mine");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new List<ShopApplicationDto>());
    }

    [HttpPost]
    public async Task<IActionResult> Apply(string name, string? description, string addressLine,
        string district, string city, string? phoneNumber, string openTime, string closeTime)
    {
        if (!IsSignedIn()) return RedirectToAction("Login", "Account", new { returnUrl = "/Sell" });

        var res = await _api.PostAsync<ShopApplicationDto>("/api/shops/apply", new
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

        TempData[res.Ok ? "ok" : "err"] = res.Ok
            ? "Application submitted. An administrator will review it shortly."
            : res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>An &lt;input type="time"&gt; posts "HH:mm"; the API expects "HH:mm:ss".</summary>
    private static string NormaliseTime(string? value)
        => string.IsNullOrWhiteSpace(value) ? "08:00:00" : (value.Count(c => c == ':') == 1 ? value + ":00" : value);
}
