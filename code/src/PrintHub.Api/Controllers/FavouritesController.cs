using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Favourites;

namespace PrintHub.Api.Controllers;

/// <summary>A customer's favourite shops (UC-11). Scoped to the authenticated user.</summary>
[ApiController]
[Route("api/favourites")]
[Authorize]
[Produces("application/json")]
public class FavouritesController : ControllerBase
{
    private readonly IFavouriteService _favourites;
    private readonly ICurrentUser _currentUser;

    public FavouritesController(IFavouriteService favourites, ICurrentUser currentUser)
    {
        _favourites = favourites;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _favourites.ListAsync(userId.Value, ct)).ToActionResult();
    }

    [HttpPost("{shopId:int}")]
    public async Task<IActionResult> Add(int shopId, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _favourites.AddAsync(userId.Value, shopId, ct))
            .ToActionResult(successMessage: "Added to favourites.");
    }

    [HttpDelete("{shopId:int}")]
    public async Task<IActionResult> Remove(int shopId, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _favourites.RemoveAsync(userId.Value, shopId, ct))
            .ToActionResult(successMessage: "Removed from favourites.");
    }
}
