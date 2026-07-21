using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops.Dtos;

/// <summary>Owner's view of one rate card entry (a shop's offering of a service type).</summary>
public record RateCardEntryDto(
    int Id,
    int ServiceTypeId,
    string ServiceTypeCode,
    string ServiceTypeName,
    string ServiceGroup,
    string PricingModel,
    decimal UnitPrice,
    decimal SetupFee,
    int MinQuantity,
    int LeadTimeMinutes,
    bool IsActive,
    IEnumerable<PriceRuleDto> PriceRules);

public record PriceRuleDto(
    int Id,
    string RuleType,
    string OptionKey,
    decimal Multiplier,
    decimal FlatExtra,
    int? MinQuantity,
    int? MaxQuantity,
    bool IsActive);

/// <summary>Add a service to the shop's rate card (UC-27).</summary>
public record AddRateCardEntryRequest(
    int ServiceTypeId,
    decimal UnitPrice,
    decimal SetupFee,
    int MinQuantity,
    int LeadTimeMinutes);

/// <summary>Update pricing on an existing rate card entry.</summary>
public record UpdateRateCardEntryRequest(
    decimal UnitPrice,
    decimal SetupFee,
    int MinQuantity,
    int LeadTimeMinutes,
    bool IsActive);

/// <summary>Add a pricing rule (multiplier / surcharge / quantity tier) to an entry.</summary>
public record AddPriceRuleRequest(
    PriceRuleType RuleType,
    string OptionKey,
    decimal Multiplier,
    decimal FlatExtra,
    int? MinQuantity,
    int? MaxQuantity);
