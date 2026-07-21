using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// Junction between <see cref="Shop"/> and <see cref="ServiceType"/> carrying
/// its own pricing attributes — a many-to-many-with-attributes relationship and
/// the central table of the platform: it <em>is</em> the shop's rate card, read
/// on every quote computation.
/// </summary>
public class ShopService : AuditableEntity
{
    public int ShopId { get; set; }
    public int ServiceTypeId { get; set; }

    /// <summary>Price per unit of measure in VND.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Fixed fee added once per order item (e.g. machine setup).</summary>
    public decimal SetupFee { get; set; }

    public int MinQuantity { get; set; } = 1;

    /// <summary>Baseline production time per unit, used to estimate completion.</summary>
    public int LeadTimeMinutes { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public Shop Shop { get; set; } = null!;
    public ServiceType ServiceType { get; set; } = null!;
    public ICollection<PriceRule> PriceRules { get; set; } = new List<PriceRule>();
}
