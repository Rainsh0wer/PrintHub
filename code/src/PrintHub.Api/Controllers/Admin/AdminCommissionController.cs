using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Platform;
using PrintHub.Application.Features.Platform.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Platform-wide money settings (UC-39): commission and cancellation fee.</summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminCommissionController : ControllerBase
{
    private readonly IPlatformSettingsService _settings;

    public AdminCommissionController(IPlatformSettingsService settings) => _settings = settings;

    [HttpGet("commission")]
    public async Task<IActionResult> Get(CancellationToken ct)
        => (await _settings.GetCommissionAsync(ct)).ToActionResult();

    [HttpPut("commission")]
    public async Task<IActionResult> Set(SetCommissionRequest request, CancellationToken ct)
        => (await _settings.SetCommissionAsync(request.Rate, ct)).ToActionResult(successMessage: "Commission rate updated.");

    /// <summary>BR-47 — the share a shop keeps when an accepted order is cancelled.</summary>
    [HttpGet("cancellation-fee")]
    public async Task<IActionResult> GetCancellationFee(CancellationToken ct)
        => (await _settings.GetCancellationFeeAsync(ct)).ToActionResult();

    [HttpPut("cancellation-fee")]
    public async Task<IActionResult> SetCancellationFee(SetCancellationFeeRequest request, CancellationToken ct)
        => (await _settings.SetCancellationFeeAsync(request.Rate, ct)).ToActionResult(successMessage: "Cancellation fee updated.");
}
