using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Features.Reports;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers;

/// <summary>
/// Reports (UC-30 shop revenue, UC-42 platform). These endpoints demonstrate
/// content negotiation: on success they return the flat report DTO directly, so the
/// framework renders JSON, XML, or CSV per the Accept header. Failures fall back to
/// the standard ApiResponse envelope.
/// </summary>
[ApiController]
[Route("api")]
[Authorize]
[Produces("application/json", "application/xml", "text/csv")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports) => _reports = reports;

    /// <summary>UC-30 — a shop's revenue/commission report (owner only).</summary>
    [HttpGet("shops/{shopId:int}/reports/revenue")]
    public async Task<IActionResult> ShopRevenue(int shopId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _reports.GetShopRevenueAsync(shopId, from, to, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToActionResult();
    }

    /// <summary>UC-42 — the platform-wide transaction/revenue report (admin).</summary>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("reports/platform")]
    public async Task<IActionResult> Platform([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _reports.GetPlatformAsync(from, to, ct);
        return result.IsSuccess ? Ok(result.Value) : result.ToActionResult();
    }
}
