using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A single pricing adjustment on a rate card entry — a paper-type multiplier,
/// a colour surcharge, a duplex adjustment, a material rate, a quality-profile
/// multiplier, or a quantity-tier discount. The Quote Engine applies the
/// matching rules to compute an effective unit price.
/// </summary>
public class PriceRule : BaseEntity
{
    public int ShopServiceId { get; set; }

    public PriceRuleType RuleType { get; set; }

    /// <summary>The option this rule applies to, e.g. "A3", "Color", "Duplex", "PLA", "Fine".</summary>
    public string OptionKey { get; set; } = null!;

    /// <summary>Multiplicative factor applied to the base price; 1.0 for no effect.</summary>
    public decimal Multiplier { get; set; } = 1m;

    /// <summary>Additive amount in VND applied when this option is selected.</summary>
    public decimal FlatExtra { get; set; }

    // Quantity band for tier rules; null for non-tier rules.
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }

    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ShopService ShopService { get; set; } = null!;
}
