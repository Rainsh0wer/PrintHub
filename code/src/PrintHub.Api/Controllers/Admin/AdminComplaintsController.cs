using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Complaints;
using PrintHub.Application.Features.Complaints.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Platform adjudication of escalated complaints (UC-41); the ruling is final.</summary>
[ApiController]
[Route("api/admin/complaints")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaints;
    private readonly ICurrentUser _currentUser;

    public AdminComplaintsController(IComplaintService complaints, ICurrentUser currentUser)
    {
        _complaints = complaints;
        _currentUser = currentUser;
    }

    /// <summary>UC-41 — list escalated complaints awaiting adjudication.</summary>
    [HttpGet]
    public async Task<IActionResult> Escalated([FromQuery] PageRequest page, CancellationToken ct)
        => (await _complaints.ListEscalatedAsync(page, ct)).ToActionResult();

    /// <summary>UC-41 — final ruling: uphold a refund or reject the complaint.</summary>
    [HttpPut("{id:int}/adjudicate")]
    public async Task<IActionResult> Adjudicate(int id, AdjudicateComplaintRequest request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId;
        if (adminId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _complaints.AdjudicateAsync(adminId.Value, id, request, ct)).ToActionResult(successMessage: "Ruling recorded.");
    }
}
