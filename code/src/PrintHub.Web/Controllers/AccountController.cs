using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Features.Auth.Dtos;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Web.Services;

namespace PrintHub.Web.Controllers;

public class AccountController : Controller
{
    private readonly PrintHubApiClient _api;
    private readonly IConfiguration _configuration;

    public AccountController(PrintHubApiClient api, IConfiguration configuration)
    {
        _api = api;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl, string? error)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.GoogleLoginUrl = $"{ApiBaseUrl()}/api/auth/google/login";
        if (error == "google") ViewBag.Error = "Google sign-in failed. Please try again or use email and password.";
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
        await StoreShopIdsAsync();
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

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var res = await _api.PostAsync<ForgotPasswordResponse>("/api/auth/forgot-password", new ForgotPasswordRequest(email));
        ViewBag.Email = email;
        if (res.Ok && res.Data is not null)
        {
            ViewBag.Message = res.Data.Message;
            ViewBag.DevToken = res.Data.ResetToken;   // dev builds return the token directly
        }
        else ViewBag.Error = res.Error ?? "Could not process the request.";
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? email, string? token)
    {
        ViewBag.Email = email;
        ViewBag.Token = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string email, string token, string newPassword, string confirmPassword)
    {
        ViewBag.Email = email;
        ViewBag.Token = token;
        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "The two passwords do not match.";
            return View();
        }

        var res = await _api.PostAsync<object>("/api/auth/reset-password", new ResetPasswordRequest(email, token, newPassword));
        if (!res.Ok)
        {
            ViewBag.Error = res.Error ?? "This reset link is invalid or has expired.";
            return View();
        }

        TempData["ok"] = "Your password has been reset. Please sign in.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public async Task<IActionResult> External(string? access, string? refresh)
    {
        if (string.IsNullOrEmpty(access)) return RedirectToAction("Login");

        var s = HttpContext.Session;
        s.SetString(SessionKeys.AccessToken, access);
        if (!string.IsNullOrEmpty(refresh)) s.SetString(SessionKeys.RefreshToken, refresh);

        var me = await _api.GetAsync<ProfileDto>("/api/users/me");
        if (!me.Ok || me.Data is null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", new { error = "google" });
        }

        s.SetString(SessionKeys.UserName, me.Data.FullName);
        s.SetString(SessionKeys.UserEmail, me.Data.Email);
        s.SetString(SessionKeys.UserRole, me.Data.Role);
        if (!string.IsNullOrEmpty(me.Data.AvatarUrl))
            s.SetString(SessionKeys.UserAvatar, me.Data.AvatarUrl);

        await StoreShopIdsAsync();
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
        if (!string.IsNullOrEmpty(auth.User.AvatarUrl))
            s.SetString(SessionKeys.UserAvatar, auth.User.AvatarUrl);
    }

    private async Task StoreShopIdsAsync()
    {
        var me = await _api.GetAsync<MeDto>("/api/auth/me");
        if (me.Ok && me.Data?.ShopIds is { Length: > 0 } ids)
            HttpContext.Session.SetString(SessionKeys.ShopIds, string.Join(",", ids));
    }

    private string ApiBaseUrl() => (_configuration["Api:BaseUrl"] ?? "http://localhost:5080").TrimEnd('/');
}
