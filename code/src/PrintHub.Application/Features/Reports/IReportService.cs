using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Reports.Dtos;

namespace PrintHub.Application.Features.Reports;

/// <summary>
/// Reporting use cases. Figures are computed from completed orders. The returned
/// DTOs are flat so the API can serve them as JSON, XML, or CSV by content
/// negotiation (UC-30 shop revenue, UC-42 platform report).
/// </summary>
public interface IReportService
{
    /// <summary>UC-30 — a shop's revenue/commission summary over an optional window (owner only).</summary>
    Task<Result<ShopRevenueReportDto>> GetShopRevenueAsync(int shopId, DateTime? from, DateTime? to, CancellationToken ct = default);

    /// <summary>UC-42 — the platform-wide transaction/revenue summary over an optional window (admin).</summary>
    Task<Result<PlatformReportDto>> GetPlatformAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
}
