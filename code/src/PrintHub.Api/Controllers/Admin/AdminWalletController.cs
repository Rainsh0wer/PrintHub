using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Wallet;
using PrintHub.Application.Features.Wallet.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>
/// Admin confirmation of wallet top-ups (UC-21). Once a bank transfer matching a
/// pending reference is seen, the administrator confirms it here, which is the
/// single point at which a top-up credits the customer's balance.
/// </summary>
[ApiController]
[Route("api/admin/wallet")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminWalletController : ControllerBase
{
    private readonly IWalletService _wallet;
    private readonly ICurrentUser _currentUser;

    public AdminWalletController(IWalletService wallet, ICurrentUser currentUser)
    {
        _wallet = wallet;
        _currentUser = currentUser;
    }

    /// <summary>Confirm a matched transfer by reference code, crediting the wallet.</summary>
    [HttpPut("topups/{refCode}/confirm")]
    public async Task<IActionResult> Confirm(string refCode, ConfirmTopUpRequest request, CancellationToken ct)
    {
        var adminId = _currentUser.UserId;
        if (adminId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _wallet.ConfirmTopUpAsync(adminId.Value, refCode, request, ct))
            .ToActionResult(successMessage: "Top-up confirmed and wallet credited.");
    }
}
