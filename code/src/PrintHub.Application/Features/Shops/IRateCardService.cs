using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Owner management of a shop's rate card — its offered services and pricing
/// rules (UC-27). This is the ShopService × ServiceType junction, read on every
/// quote. Owner-scoped (BR-74).
/// </summary>
public interface IRateCardService
{
    Task<Result<IReadOnlyList<RateCardEntryDto>>> GetRateCardAsync(int shopId, CancellationToken ct = default);
    Task<Result<RateCardEntryDto>> AddEntryAsync(int shopId, AddRateCardEntryRequest request, CancellationToken ct = default);
    Task<Result<RateCardEntryDto>> UpdateEntryAsync(int shopId, int entryId, UpdateRateCardEntryRequest request, CancellationToken ct = default);
    Task<Result> AddPriceRuleAsync(int shopId, int entryId, AddPriceRuleRequest request, CancellationToken ct = default);
    Task<Result> RemovePriceRuleAsync(int shopId, int entryId, int priceRuleId, CancellationToken ct = default);
}
