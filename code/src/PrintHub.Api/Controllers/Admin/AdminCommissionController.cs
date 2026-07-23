using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Platform;
using PrintHub.Application.Features.Platform.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.Admin;

/// <summary>Platform commission configuration (UC-39).</summary>
[ApiController]
[Route("api/admin/commission")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Produces("application/json")]
public class AdminCommissionController : ControllerBase
{
    private readonly IPlatformSettingsService _settings;

    public AdminCommissionController(IPlatformSettingsService settings) => _settings = settings;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => (await _settings.GetCommissionAsync(ct)).ToActionResult();

    [HttpPut]
    public async Task<IActionResult> Set(SetCommissionRequest request, CancellationToken ct)
        => (await _settings.SetCommissionAsync(request.Rate, ct)).ToActionResult(successMessage: "Commission rate updated.");
}
