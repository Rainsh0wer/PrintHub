using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Auth;
using PrintHub.Application.Features.Auth.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Authentication endpoints (UC-01..UC-05). The controller only receives the
/// request, calls the service, and maps the Result to a response.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _currentUser;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService auth, ICurrentUser currentUser, IConfiguration configuration)
    {
        _auth = auth;
        _currentUser = currentUser;
        _configuration = configuration;
    }

    /// <summary>UC-01 — register a new customer account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
        => (await _auth.RegisterAsync(request, ct)).ToActionResult(StatusCodes.Status201Created, "Account created.");

    /// <summary>UC-02 — authenticate and receive an access/refresh token pair.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
        => (await _auth.LoginAsync(request, ct)).ToActionResult();

    /// <summary>Exchange a valid refresh token for a fresh token pair (rotating).</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(RefreshRequest request, CancellationToken ct)
        => (await _auth.RefreshAsync(request, ct)).ToActionResult();

    /// <summary>UC-04 — revoke the presented refresh token server-side.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken ct)
        => (await _auth.LogoutAsync(request.RefreshToken, ct)).ToActionResult(successMessage: "Signed out.");

    /// <summary>UC-05 — change the current user's password.</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));

        return (await _auth.ChangePasswordAsync(userId.Value, request, ct))
            .ToActionResult(successMessage: "Your password has been changed.");
    }

    /// <summary>UC-03 — request a password reset token for an email.</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
        => (await _auth.ForgotPasswordAsync(request, ct)).ToActionResult();

    /// <summary>UC-03 — reset the password using the emailed token.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
        => (await _auth.ResetPasswordAsync(request, ct)).ToActionResult(successMessage: "Your password has been reset. Please log in.");

    /// <summary>UC-02 external — begin the Google OAuth handshake.</summary>
    [HttpGet("google/login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        if (!GoogleConfigured())
            return BadRequest(ApiResponse.Fail("Google sign-in is not configured on the server."));

        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>OAuth callback: provisions/looks up the account, then hands tokens back to the web app.</summary>
    [HttpGet("google/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback(CancellationToken ct)
    {
        var webBase = _configuration["Web:BaseUrl"] ?? "http://localhost:5100";
        var result = await HttpContext.AuthenticateAsync("External");
        if (!result.Succeeded || result.Principal is null)
            return Redirect($"{webBase}/Account/Login?error=google");

        var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value ?? email;
        await HttpContext.SignOutAsync("External");

        var auth = await _auth.LoginExternalAsync(email, name, ct);
        if (!auth.IsSuccess || auth.Value is null)
            return Redirect($"{webBase}/Account/Login?error=google");

        var query = $"access={Uri.EscapeDataString(auth.Value.AccessToken)}&refresh={Uri.EscapeDataString(auth.Value.RefreshToken)}";
        return Redirect($"{webBase}/Account/External?{query}");
    }

    private bool GoogleConfigured() =>
        !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientId"])
        && !string.IsNullOrWhiteSpace(_configuration["Authentication:Google:ClientSecret"]);

    /// <summary>Returns the identity carried by the current access token.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
        => Ok(ApiResponse.Ok(new
        {
            id = _currentUser.UserId,
            email = _currentUser.Email,
            role = _currentUser.Role?.ToString(),
            shopIds = _currentUser.ShopIds
        }));
}
