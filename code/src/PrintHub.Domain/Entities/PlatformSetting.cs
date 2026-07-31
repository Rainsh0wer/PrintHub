using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>Singleton platform configuration (one row): commission and cancellation fee.</summary>
public class PlatformSetting : BaseEntity
{
    public decimal CommissionRate { get; set; }

    /// <summary>
    /// Fraction of the order total kept by the shop when a customer cancels an order the
    /// shop had already accepted (BR-47), since capacity was committed by then.
    /// </summary>
    public decimal CancellationFeeRate { get; set; }

    public DateTime UpdatedAt { get; set; }
}
