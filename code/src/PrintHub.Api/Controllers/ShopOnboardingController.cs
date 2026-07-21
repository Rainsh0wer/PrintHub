using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Shop onboarding from the applicant's side (UC-25).</summary>
[ApiController]
[Route("api/shops")]
[Authorize]
[Produces("application/json")]
public class ShopOnboardingController : ControllerBase
{
    private readonly IShopOnboardingService _onboarding;
    private readonly ICurrentUser _currentUser;

    public ShopOnboardingController(IShopOnboardingService onboarding, ICurrentUser currentUser)
    {
        _onboarding = onboarding;
        _currentUser = currentUser;
    }

    /// <summary>UC-25 — apply to open a shop (enters PendingReview).</summary>
    [HttpPost("apply")]
    public async Task<IActionResult> Apply(ShopApplicationRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _onboarding.ApplyAsync(userId.Value, request, ct))
            .ToActionResult(StatusCodes.Status201Created, "Your application has been submitted and is under review.");
    }

    /// <summary>The applicant's own shops and their onboarding status.</summary>
    [HttpGet("mine")]
    public async Task<IActionResult> Mine(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _onboarding.GetMyShopsAsync(userId.Value, ct)).ToActionResult();
    }
}
