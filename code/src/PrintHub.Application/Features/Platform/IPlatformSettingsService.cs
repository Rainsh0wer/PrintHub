using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Platform.Dtos;

namespace PrintHub.Application.Features.Platform;

/// <summary>Platform-wide settings (UC-39): the commission rate applied on completion.</summary>
public interface IPlatformSettingsService
{
    Task<decimal> GetCommissionRateAsync(CancellationToken ct = default);
    Task<Result<CommissionDto>> GetCommissionAsync(CancellationToken ct = default);
    Task<Result<CommissionDto>> SetCommissionAsync(decimal rate, CancellationToken ct = default);
}
