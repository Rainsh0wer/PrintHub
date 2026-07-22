using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Wallet;
using PrintHub.Application.Features.Wallet.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Customer wallet (UC-21 top-up request, UC-22 balance + ledger).</summary>
[ApiController]
[Route("api/wallet")]
[Authorize]
[Produces("application/json")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _wallet;
    private readonly ICurrentUser _currentUser;

    public WalletController(IWalletService wallet, ICurrentUser currentUser)
    {
        _wallet = wallet;
        _currentUser = currentUser;
    }

    /// <summary>UC-22 — balance and paged transaction ledger.</summary>
    [HttpGet("transactions")]
    public async Task<IActionResult> Transactions([FromQuery] PageRequest page, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _wallet.GetLedgerAsync(userId.Value, page, ct)).ToActionResult();
    }

    /// <summary>UC-21 — request a top-up; returns bank-transfer instructions (VietQR).</summary>
    [HttpPost("topup")]
    public async Task<IActionResult> TopUp(TopUpRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _wallet.RequestTopUpAsync(userId.Value, request, ct))
            .ToActionResult(StatusCodes.Status201Created, "Top-up requested. Complete the bank transfer to credit your wallet.");
    }
}
