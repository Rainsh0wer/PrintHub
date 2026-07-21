namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Application-facing contract for the gRPC Quote Engine. Implemented in
/// Infrastructure, which maps these DTOs to/from the protobuf messages. Returns
/// null when the engine is unreachable so the caller can fall back to an
/// indicative price (graceful degradation).
/// </summary>
public interface IQuoteEngineClient
{
    Task<QuoteEstimate?> EstimateAsync(QuoteEstimateInput input, CancellationToken ct = default);
}

public record QuoteEstimateInput(IReadOnlyList<QuoteItemInput> Items);

public record QuoteItemInput(
    string PricingModel,
    int Quantity,
    int PageCount,
    double EstimatedGrams,
    decimal UnitPrice,
    decimal SetupFee,
    int LeadTimeMinutes,
    IReadOnlyList<string> SelectedOptions,
    IReadOnlyList<QuotePriceRuleInput> Rules);

public record QuotePriceRuleInput(
    string RuleType,
    string OptionKey,
    decimal Multiplier,
    decimal FlatExtra,
    int? MinQuantity,
    int? MaxQuantity);

public record QuoteEstimate(
    decimal Subtotal,
    int EstimatedMinutes,
    IReadOnlyList<QuoteLine> Lines);

public record QuoteLine(
    string Description,
    decimal EffectiveUnitPrice,
    decimal LineTotal,
    int Minutes,
    IReadOnlyList<string> AppliedRules);
