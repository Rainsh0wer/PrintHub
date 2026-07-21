using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A price computed for a specific order configuration at a specific shop. On
/// order placement the selected quote's breakdown is snapshotted onto the order,
/// so a later rate card change never alters agreed terms.
/// </summary>
public class Quote : BaseEntity
{
    public int CustomerId { get; set; }
    public int ShopId { get; set; }

    /// <summary>Computed price before discount, in VND.</summary>
    public decimal SubTotal { get; set; }

    /// <summary>Estimated production time returned by the Quote Engine.</summary>
    public int EstimatedMinutes { get; set; }

    public double? DistanceMeters { get; set; }

    /// <summary>Itemised computation trace: which rules applied and their effect.</summary>
    public string? BreakdownJson { get; set; }

    /// <summary>True when produced by the fallback path because the Quote Engine was down.</summary>
    public bool IsIndicative { get; set; }

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User Customer { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
}
