using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Quotes;
using PrintHub.Application.Features.Quotes.Dtos;
using PrintHub.Application.Features.Vouchers;
using PrintHub.Application.Features.Vouchers.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Quote comparison (UC-13) and voucher preview (UC-14).</summary>
[ApiController]
[Route("api/quotes")]
[Authorize]
[Produces("application/json")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quotes;
    private readonly IVoucherService _vouchers;
    private readonly ICurrentUser _currentUser;

    public QuotesController(IQuoteService quotes, IVoucherService vouchers, ICurrentUser currentUser)
    {
        _quotes = quotes;
        _vouchers = vouchers;
        _currentUser = currentUser;
    }

    /// <summary>UC-13 — configure an order once and compare quotes across eligible shops.</summary>
    [HttpPost("compare")]
    public async Task<IActionResult> Compare(CompareQuotesRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _quotes.CompareAsync(userId.Value, request, ct)).ToActionResult();
    }

    /// <summary>UC-14 — apply a voucher to a quote and recompute the total (preview).</summary>
    [HttpPost("{id:int}/apply-voucher")]
    public async Task<IActionResult> ApplyVoucher(int id, ApplyVoucherRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _vouchers.ApplyToQuoteAsync(userId.Value, id, request.Code, ct)).ToActionResult();
    }
}
