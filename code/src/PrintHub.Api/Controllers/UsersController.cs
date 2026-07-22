using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Users;
using PrintHub.Application.Features.Users.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>The caller's own account profile (UC-06 view, UC-07 update).</summary>
[ApiController]
[Route("api/users")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IProfileService _profile;
    private readonly ICurrentUser _currentUser;

    public UsersController(IProfileService profile, ICurrentUser currentUser)
    {
        _profile = profile;
        _currentUser = currentUser;
    }

    /// <summary>UC-06 — view own profile.</summary>
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _profile.GetMeAsync(userId.Value, ct)).ToActionResult();
    }

    /// <summary>UC-07 — update display name, phone, default address, avatar.</summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _profile.UpdateMeAsync(userId.Value, request, ct)).ToActionResult(successMessage: "Profile updated.");
    }
}
