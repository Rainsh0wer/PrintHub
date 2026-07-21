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

    public AuthController(IAuthService auth, ICurrentUser currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
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
