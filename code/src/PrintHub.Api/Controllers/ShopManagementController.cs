using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Owner-side shop management (UC-26 profile, UC-27 rate card). Authorization is
/// scoped inside the services: the caller must own the shop in the route, so a
/// crafted shopId returns 403 rather than touching another shop's data.
/// </summary>
[ApiController]
[Route("api/shops/{shopId:int}")]
[Authorize]
[Produces("application/json")]
public class ShopManagementController : ControllerBase
{
    private readonly IShopProfileService _profile;
    private readonly IRateCardService _rateCard;

    public ShopManagementController(IShopProfileService profile, IRateCardService rateCard)
    {
        _profile = profile;
        _rateCard = rateCard;
    }

    /// <summary>UC-26 — update the shop's storefront profile.</summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(int shopId, ShopApplicationRequest request, CancellationToken ct)
        => (await _profile.UpdateProfileAsync(shopId, request, ct)).ToActionResult(successMessage: "Shop profile updated.");

    /// <summary>UC-27 — the shop's full rate card (entries + price rules).</summary>
    [HttpGet("rate-card")]
    public async Task<IActionResult> GetRateCard(int shopId, CancellationToken ct)
        => (await _rateCard.GetRateCardAsync(shopId, ct)).ToActionResult();

    /// <summary>UC-27 — add a service to the rate card.</summary>
    [HttpPost("rate-card")]
    public async Task<IActionResult> AddEntry(int shopId, AddRateCardEntryRequest request, CancellationToken ct)
        => (await _rateCard.AddEntryAsync(shopId, request, ct)).ToActionResult(StatusCodes.Status201Created, "Service added to rate card.");

    /// <summary>UC-27 — update pricing on a rate card entry.</summary>
    [HttpPut("rate-card/{entryId:int}")]
    public async Task<IActionResult> UpdateEntry(int shopId, int entryId, UpdateRateCardEntryRequest request, CancellationToken ct)
        => (await _rateCard.UpdateEntryAsync(shopId, entryId, request, ct)).ToActionResult(successMessage: "Rate card entry updated.");

    /// <summary>UC-27 — add a pricing rule to a rate card entry.</summary>
    [HttpPost("rate-card/{entryId:int}/rules")]
    public async Task<IActionResult> AddPriceRule(int shopId, int entryId, AddPriceRuleRequest request, CancellationToken ct)
        => (await _rateCard.AddPriceRuleAsync(shopId, entryId, request, ct)).ToActionResult(StatusCodes.Status201Created, "Price rule added.");

    /// <summary>UC-27 — remove a pricing rule.</summary>
    [HttpDelete("rate-card/{entryId:int}/rules/{ruleId:int}")]
    public async Task<IActionResult> RemovePriceRule(int shopId, int entryId, int ruleId, CancellationToken ct)
        => (await _rateCard.RemovePriceRuleAsync(shopId, entryId, ruleId, ct)).ToActionResult(successMessage: "Price rule removed.");
}
