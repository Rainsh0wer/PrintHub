using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.ServiceTypes;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Manages a shop's rate card. Every method is scoped to the owning shop (BR-74).
/// Enforces service-type existence, one-entry-per-service-type, non-negative
/// pricing (BR-71), and non-overlapping quantity tiers (BR-72).
/// </summary>
public class RateCardService : IRateCardService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public RateCardService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<RateCardEntryDto>>> GetRateCardAsync(int shopId, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<IReadOnlyList<RateCardEntryDto>>.Forbidden("You do not have permission to access this shop's data.");

        var entries = await _uow.Repository<ShopService>().ListAsync(new RateCardByShopSpecification(shopId), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<RateCardEntryDto>>(entries));
    }

    public async Task<Result<RateCardEntryDto>> AddEntryAsync(int shopId, AddRateCardEntryRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<RateCardEntryDto>.Forbidden("You do not have permission to access this shop's data.");

        var serviceTypeExists = await _uow.Repository<ServiceType>()
            .AnyAsync(new ActiveServiceTypeByIdSpecification(request.ServiceTypeId), ct);
        if (!serviceTypeExists)
            return Result<RateCardEntryDto>.Fail("This service type is not available in the platform catalogue.");

        var entries = _uow.Repository<ShopService>();
        if (await entries.AnyAsync(new ShopServiceByShopAndTypeSpecification(shopId, request.ServiceTypeId), ct))
            return Result<RateCardEntryDto>.Conflict("This shop already offers this service.");

        var entry = new ShopService
        {
            ShopId = shopId,
            ServiceTypeId = request.ServiceTypeId,
            UnitPrice = request.UnitPrice,
            SetupFee = request.SetupFee,
            MinQuantity = request.MinQuantity,
            LeadTimeMinutes = request.LeadTimeMinutes,
            IsActive = true
        };
        await entries.AddAsync(entry, ct);
        await _uow.SaveChangesAsync(ct);

        // Re-load with the service type so the response is complete.
        var saved = await entries.FirstOrDefaultAsync(new ShopServiceWithTypeByIdSpecification(entry.Id), ct);
        return Result.Success(_mapper.Map<RateCardEntryDto>(saved!));
    }

    public async Task<Result<RateCardEntryDto>> UpdateEntryAsync(int shopId, int entryId, UpdateRateCardEntryRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<RateCardEntryDto>.Forbidden("You do not have permission to access this shop's data.");

        var entries = _uow.Repository<ShopService>();
        var entry = await entries.FirstOrDefaultAsync(new ShopServiceWithTypeByIdSpecification(entryId), ct);
        if (entry is null || entry.ShopId != shopId)
            return Result<RateCardEntryDto>.NotFound("This rate card entry could not be found.");

        entry.UnitPrice = request.UnitPrice;
        entry.SetupFee = request.SetupFee;
        entry.MinQuantity = request.MinQuantity;
        entry.LeadTimeMinutes = request.LeadTimeMinutes;
        entry.IsActive = request.IsActive;
        entries.Update(entry);

        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<RateCardEntryDto>(entry));
    }

    public async Task<Result> AddPriceRuleAsync(int shopId, int entryId, AddPriceRuleRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var entries = _uow.Repository<ShopService>();
        var entry = await entries.FirstOrDefaultAsync(new ShopServiceByIdSpecification(entryId), ct);
        if (entry is null || entry.ShopId != shopId)
            return Result.NotFound("This rate card entry could not be found.");

        // BR-72: quantity tiers on the same entry must not overlap.
        if (request.RuleType == PriceRuleType.QuantityTier)
        {
            var overlap = entry.PriceRules
                .Where(r => r.RuleType == PriceRuleType.QuantityTier && r.IsActive)
                .Any(r => Overlaps(r.MinQuantity, r.MaxQuantity, request.MinQuantity, request.MaxQuantity));
            if (overlap)
                return Result.Conflict("Quantity tiers overlap. Please review the affected bands.");
        }

        await _uow.Repository<PriceRule>().AddAsync(new PriceRule
        {
            ShopServiceId = entryId,
            RuleType = request.RuleType,
            OptionKey = request.OptionKey,
            Multiplier = request.Multiplier,
            FlatExtra = request.FlatExtra,
            MinQuantity = request.MinQuantity,
            MaxQuantity = request.MaxQuantity,
            IsActive = true
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> RemovePriceRuleAsync(int shopId, int entryId, int priceRuleId, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var entries = _uow.Repository<ShopService>();
        var entry = await entries.FirstOrDefaultAsync(new ShopServiceByIdSpecification(entryId), ct);
        if (entry is null || entry.ShopId != shopId)
            return Result.NotFound("This rate card entry could not be found.");

        var rule = entry.PriceRules.FirstOrDefault(r => r.Id == priceRuleId);
        if (rule is null)
            return Result.NotFound("This price rule could not be found.");

        _uow.Repository<PriceRule>().Remove(rule);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static bool Overlaps(int? aMin, int? aMax, int? bMin, int? bMax)
    {
        var lowA = aMin ?? int.MinValue;
        var highA = aMax ?? int.MaxValue;
        var lowB = bMin ?? int.MinValue;
        var highB = bMax ?? int.MaxValue;
        return lowA <= highB && lowB <= highA;
    }
}
