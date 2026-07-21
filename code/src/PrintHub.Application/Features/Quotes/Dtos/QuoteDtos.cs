using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Quotes.Dtos;

public enum QuoteSortBy
{
    Price = 0,
    Time = 1,
    Rating = 2
}

/// <summary>Configure an order once and compare quotes across eligible shops (UC-13).</summary>
public class CompareQuotesRequest
{
    public List<CompareItemInput> Items { get; set; } = new();
    /// <summary>Optional: restrict the comparison to a single shop.</summary>
    public int? ShopId { get; set; }
    public QuoteSortBy SortBy { get; set; } = QuoteSortBy.Price;
}

public class CompareItemInput
{
    public int ServiceTypeId { get; set; }
    public int Quantity { get; set; } = 1;
    public int? PageCount { get; set; }
    public string? PaperType { get; set; }
    public ColorMode? ColorMode { get; set; }
    public Sides? Sides { get; set; }
    public string? BindingType { get; set; }
    public string? MaterialName { get; set; }
    public string? QualityProfile { get; set; }
    public decimal? EstimatedGrams { get; set; }
}

public record QuoteComparisonDto(
    int QuoteId,
    int ShopId,
    string ShopName,
    string District,
    double Rating,
    int RatingCount,
    decimal Total,
    int EstimatedMinutes,
    double? DistanceMeters,
    bool IsIndicative,
    IEnumerable<QuoteLineDto> Lines);

public record QuoteLineDto(
    string Description,
    decimal EffectiveUnitPrice,
    decimal LineTotal,
    int Minutes,
    IEnumerable<string> AppliedRules);
