using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Platform.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Platform;

public class PlatformSettingsService : IPlatformSettingsService
{
    private const decimal DefaultCommissionRate = 0.10m;

    private readonly IUnitOfWork _uow;

    public PlatformSettingsService(IUnitOfWork uow) => _uow = uow;

    public async Task<decimal> GetCommissionRateAsync(CancellationToken ct = default)
        => (await GetOrCreateAsync(ct)).CommissionRate;

    public async Task<Result<CommissionDto>> GetCommissionAsync(CancellationToken ct = default)
    {
        var setting = await GetOrCreateAsync(ct);
        return Result.Success(new CommissionDto(setting.CommissionRate, setting.UpdatedAt));
    }

    public async Task<Result<CommissionDto>> SetCommissionAsync(decimal rate, CancellationToken ct = default)
    {
        if (rate is < 0m or > 1m)
            return Result<CommissionDto>.Fail("Commission rate must be between 0 and 1 (e.g. 0.10 = 10%).");

        var setting = await GetOrCreateAsync(ct);
        setting.CommissionRate = rate;
        setting.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<PlatformSetting>().Update(setting);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(new CommissionDto(setting.CommissionRate, setting.UpdatedAt));
    }

    private async Task<PlatformSetting> GetOrCreateAsync(CancellationToken ct)
    {
        var existing = await _uow.Repository<PlatformSetting>().ListAllAsync(ct);
        if (existing.Count > 0) return existing[0];

        var setting = new PlatformSetting { CommissionRate = DefaultCommissionRate, UpdatedAt = DateTime.UtcNow };
        await _uow.Repository<PlatformSetting>().AddAsync(setting, ct);
        await _uow.SaveChangesAsync(ct);
        return setting;
    }
}
