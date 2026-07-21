using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops.Dtos;

/// <summary>
/// OData projection of a shop. Deliberately a mutable class with an Id key so
/// OData can shape it ($select) and query it ($filter/$orderby). Enum values are
/// kept as enums (not stringified) so the whole projection translates to SQL.
/// </summary>
public class ShopODataDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public ShopStatus Status { get; set; }
    public double RatingAverage { get; set; }
    public int RatingCount { get; set; }

    /// <summary>Rate card as a complex-type collection (always projected; trimmed by $select).</summary>
    public List<ShopServiceODataDto> Services { get; set; } = new();
}

/// <summary>Complex type (no key) used inside <see cref="ShopODataDto.Services"/>.</summary>
public class ShopServiceODataDto
{
    public string ServiceTypeName { get; set; } = null!;
    public ServiceGroup ServiceGroup { get; set; }
    public decimal UnitPrice { get; set; }
    public int LeadTimeMinutes { get; set; }
}
