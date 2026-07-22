using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Reviews;
using PrintHub.Application.Features.Reviews.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Reviews (UC-23): a customer rates a completed order; shop reviews are public.</summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviews;
    private readonly ICurrentUser _currentUser;

    public ReviewsController(IReviewService reviews, ICurrentUser currentUser)
    {
        _reviews = reviews;
        _currentUser = currentUser;
    }

    /// <summary>UC-23 — rate + review a completed order (one per order).</summary>
    [Authorize]
    [HttpPost("orders/{orderId:int}/review")]
    public async Task<IActionResult> Create(int orderId, CreateReviewRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _reviews.CreateAsync(userId.Value, orderId, request, ct))
            .ToActionResult(StatusCodes.Status201Created, "Thank you for your review.");
    }

    /// <summary>UC-23 — a shop's public reviews (paged).</summary>
    [AllowAnonymous]
    [HttpGet("shops/{shopId:int}/reviews")]
    public async Task<IActionResult> ListForShop(int shopId, [FromQuery] PageRequest page, CancellationToken ct)
        => (await _reviews.ListForShopAsync(shopId, page, ct)).ToActionResult();
}
