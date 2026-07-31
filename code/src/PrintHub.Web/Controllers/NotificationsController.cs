using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Notifications.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

/// <summary>UC-08 — the caller's notification centre.</summary>
public class NotificationsController : Controller
{
    private readonly PrintHubApiClient _api;
    public NotificationsController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeys.AccessToken)))
            return RedirectToAction("Login", "Account", new { returnUrl = "/Notifications" });

        var res = await _api.GetAsync<PagedResult<NotificationDto>>("/api/notifications?PageSize=50");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<NotificationDto>());
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllRead()
    {
        var res = await _api.PutAsync<object>("/api/notifications/read", new { ids = (int[]?)null });
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? "All notifications marked as read." : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
