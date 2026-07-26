using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AdminUsersController : ConsoleBase
{
    private readonly PrintHubApiClient _api;
    public AdminUsersController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? q)
    {
        if (!IsAdmin()) return RedirectToAction("Login", "Account", new { returnUrl = "/AdminUsers" });
        var path = "/api/admin/users?PageSize=50" + (string.IsNullOrWhiteSpace(q) ? "" : $"&q={Uri.EscapeDataString(q)}");
        var res = await _api.GetAsync<PagedResult<UserListItemDto>>(path);
        ViewBag.Query = q;
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data ?? new PagedResult<UserListItemDto>());
    }

    [HttpPost] public Task<IActionResult> Lock(int id) => Act($"/api/admin/users/{id}/lock", "Account locked.");
    [HttpPost] public Task<IActionResult> Unlock(int id) => Act($"/api/admin/users/{id}/unlock", "Account unlocked.");

    private async Task<IActionResult> Act(string path, string ok)
    {
        var res = await _api.PutAsync<object>(path, null);
        TempData[res.Ok ? "ok" : "err"] = res.Ok ? ok : res.Error;
        return RedirectToAction(nameof(Index));
    }
}
