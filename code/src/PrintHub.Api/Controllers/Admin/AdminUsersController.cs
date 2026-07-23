using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Admin account management (UC-38): search, lock, unlock.</summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _users;

    public AdminUsersController(IAdminUserService users) => _users = users;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] UserRole? role,
        [FromQuery] UserStatus? status, [FromQuery] PageRequest page, CancellationToken ct)
        => (await _users.SearchAsync(q, role, status, page, ct)).ToActionResult();

    [HttpPut("{id:int}/lock")]
    public async Task<IActionResult> Lock(int id, CancellationToken ct)
        => (await _users.LockAsync(id, ct)).ToActionResult(successMessage: "Account locked.");

    [HttpPut("{id:int}/unlock")]
    public async Task<IActionResult> Unlock(int id, CancellationToken ct)
        => (await _users.UnlockAsync(id, ct)).ToActionResult(successMessage: "Account unlocked.");
}
