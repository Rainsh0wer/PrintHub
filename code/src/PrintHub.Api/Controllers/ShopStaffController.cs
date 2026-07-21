using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Owner management of shop staff (UC-29). Owner-scoped inside the service.</summary>
[ApiController]
[Route("api/shops/{shopId:int}/staff")]
[Authorize]
[Produces("application/json")]
public class ShopStaffController : ControllerBase
{
    private readonly IShopStaffService _staff;

    public ShopStaffController(IShopStaffService staff) => _staff = staff;

    [HttpGet]
    public async Task<IActionResult> List(int shopId, CancellationToken ct)
        => (await _staff.ListAsync(shopId, ct)).ToActionResult();

    [HttpPost]
    public async Task<IActionResult> Grant(int shopId, GrantStaffRequest request, CancellationToken ct)
        => (await _staff.GrantAsync(shopId, request, ct)).ToActionResult(StatusCodes.Status201Created, "Staff access granted.");

    [HttpDelete("{staffId:int}")]
    public async Task<IActionResult> Revoke(int shopId, int staffId, CancellationToken ct)
        => (await _staff.RevokeAsync(shopId, staffId, ct)).ToActionResult(successMessage: "Staff access revoked.");
}
