using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints;
using PrintHub.Application.Features.Complaints.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Complaint workflow (UC-24 customer, UC-35 shop response). Customer steps are
/// scoped to the complaint owner and shop response to the shop, both enforced in
/// the service; admin adjudication lives in Admin/AdminComplaintsController.
/// </summary>
[ApiController]
[Route("api/complaints")]
[Authorize]
[Produces("application/json")]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaints;
    private readonly ICurrentUser _currentUser;

    public ComplaintsController(IComplaintService complaints, ICurrentUser currentUser)
    {
        _complaints = complaints;
        _currentUser = currentUser;
    }

    /// <summary>UC-24 — raise a complaint on a completed order.</summary>
    [HttpPost]
    public async Task<IActionResult> Raise(RaiseComplaintRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _complaints.RaiseAsync(userId.Value, request, ct))
            .ToActionResult(StatusCodes.Status201Created, "Complaint submitted.");
    }

    /// <summary>UC-24 — the caller's complaints and their status.</summary>
    [HttpGet("mine")]
    public async Task<IActionResult> Mine([FromQuery] PageRequest page, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _complaints.ListMineAsync(userId.Value, page, ct)).ToActionResult();
    }

    /// <summary>UC-35 — the shop proposes a reprint or refund resolution.</summary>
    [HttpPut("{id:int}/respond")]
    public async Task<IActionResult> Respond(int id, RespondComplaintRequest request, CancellationToken ct)
        => (await _complaints.RespondAsync(id, request, ct)).ToActionResult(successMessage: "Response submitted.");

    /// <summary>UC-24 — the customer accepts the proposed resolution.</summary>
    [HttpPut("{id:int}/accept")]
    public async Task<IActionResult> Accept(int id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _complaints.AcceptAsync(userId.Value, id, ct)).ToActionResult(successMessage: "Resolution accepted.");
    }

    /// <summary>UC-24 — the customer rejects and escalates to the platform.</summary>
    [HttpPut("{id:int}/escalate")]
    public async Task<IActionResult> Escalate(int id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _complaints.EscalateAsync(userId.Value, id, ct)).ToActionResult(successMessage: "Complaint escalated.");
    }
}
