using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class ProfileController : Controller
{
    private readonly PrintHubApiClient _api;
    public ProfileController(PrintHubApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeys.AccessToken)))
            return RedirectToAction("Login", "Account", new { returnUrl = "/Profile" });
        var res = await _api.GetAsync<ProfileDto>("/api/users/me");
        ViewBag.Error = res.Ok ? null : res.Error;
        return View(res.Data);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string fullName, string? phoneNumber, string? defaultAddress, string? avatarUrl)
    {
        var res = await _api.PutAsync<ProfileDto>("/api/users/me", new { fullName, phoneNumber, defaultAddress, avatarUrl });
        if (res.Ok && res.Data is not null)
        {
            HttpContext.Session.SetString(SessionKeys.UserName, res.Data.FullName);
            TempData["ok"] = "Profile updated.";
        }
        else TempData["err"] = res.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>UC-05 — changing the password revokes every refresh token (BR-9), so the
    /// session is cleared and the user is sent back to sign in.</summary>
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
    {
        var res = await _api.PostAsync<object>("/api/auth/change-password",
            new { currentPassword, newPassword, confirmNewPassword });

        if (!res.Ok)
        {
            TempData["err"] = res.Error;
            return RedirectToAction(nameof(Index));
        }

        HttpContext.Session.Clear();
        TempData["ok"] = "Password changed. Please sign in again.";
        return RedirectToAction("Login", "Account");
    }
}
