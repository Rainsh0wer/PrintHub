using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Quotes;
using PrintHub.Application.Features.Quotes.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Quote comparison (UC-13) — the platform's distinguishing capability.</summary>
[ApiController]
[Route("api/quotes")]
[Authorize]
[Produces("application/json")]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quotes;
    private readonly ICurrentUser _currentUser;

    public QuotesController(IQuoteService quotes, ICurrentUser currentUser)
    {
        _quotes = quotes;
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
}
