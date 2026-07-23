using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Auth.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AccountController : Controller
{
    private readonly PrintHubApiClient _api;

    public AccountController(PrintHubApiClient api) => _api = api;

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl)
    {
        var res = await _api.PostAsync<AuthResponse>("/api/auth/login", new LoginRequest(email, password));
        if (!res.Ok || res.Data is null)
        {
            ViewBag.Error = res.Error ?? "Sign in failed.";
            ViewBag.Email = email;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        StoreSession(res.Data);
        return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string? phoneNumber, string password, string confirmPassword)
    {
        var res = await _api.PostAsync<AuthResponse>("/api/auth/register",
            new RegisterRequest(fullName, email, phoneNumber, password, confirmPassword));
        if (!res.Ok || res.Data is null)
        {
            ViewBag.Error = res.Error ?? "Registration failed.";
            ViewBag.FullName = fullName; ViewBag.Email = email; ViewBag.Phone = phoneNumber;
            return View();
        }
        StoreSession(res.Data);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        var refresh = HttpContext.Session.GetString(SessionKeys.RefreshToken);
        if (!string.IsNullOrEmpty(refresh))
            await _api.PostAsync<object>("/api/auth/logout", new { refreshToken = refresh });
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private void StoreSession(AuthResponse auth)
    {
        var s = HttpContext.Session;
        s.SetString(SessionKeys.AccessToken, auth.AccessToken);
        s.SetString(SessionKeys.RefreshToken, auth.RefreshToken);
        s.SetString(SessionKeys.UserName, auth.User.FullName);
        s.SetString(SessionKeys.UserEmail, auth.User.Email);
        s.SetString(SessionKeys.UserRole, auth.User.Role);
    }
}
