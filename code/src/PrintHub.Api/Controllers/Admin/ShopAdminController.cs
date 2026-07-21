using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Shops;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>
/// Administrator governance of shops (UC-36, UC-37). Admin-only; drives the shop
/// onboarding state machine.
/// </summary>
[ApiController]
[Route("api/admin/shops")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class ShopAdminController : ControllerBase
{
    private readonly IShopAdminService _admin;

    public ShopAdminController(IShopAdminService admin) => _admin = admin;

    /// <summary>UC-36 — list shop applications awaiting review.</summary>
    [HttpGet("applications")]
    public async Task<IActionResult> PendingApplications(CancellationToken ct)
        => (await _admin.ListPendingApplicationsAsync(ct)).ToActionResult();

    /// <summary>UC-36 — approve an application (activates the shop, elevates the owner).</summary>
    [HttpPut("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
        => (await _admin.ApproveAsync(id, ct)).ToActionResult(successMessage: "Shop approved and activated.");

    /// <summary>UC-36 — reject an application with a mandatory reason.</summary>
    [HttpPut("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, ReasonRequest request, CancellationToken ct)
        => (await _admin.RejectAsync(id, request.Reason, ct)).ToActionResult(successMessage: "Application rejected.");

    /// <summary>UC-37 — suspend an active shop with a mandatory reason.</summary>
    [HttpPut("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id, ReasonRequest request, CancellationToken ct)
        => (await _admin.SuspendAsync(id, request.Reason, ct))
            .ToActionResult(successMessage: "Shop suspended. In-progress orders continue to completion.");

    /// <summary>UC-37 — reinstate a suspended shop.</summary>
    [HttpPut("{id:int}/reinstate")]
    public async Task<IActionResult> Reinstate(int id, CancellationToken ct)
        => (await _admin.ReinstateAsync(id, ct)).ToActionResult(successMessage: "Shop reinstated.");
}
