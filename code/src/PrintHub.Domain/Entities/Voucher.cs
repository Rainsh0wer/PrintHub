using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A platform-funded promotional voucher. Its redemption count is incremented
/// only on successful order placement, not when applied at checkout.
/// </summary>
public class Voucher : AuditableEntity
{
    public string Code { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    /// <summary>Max redemptions per user (null = unlimited within the global UsageLimit).</summary>
    public int? PerUserLimit { get; set; }

    public VoucherDiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }

    public decimal MinOrderAmount { get; set; }

    /// <summary>Cap on the discount when the type is Percent.</summary>
    public decimal? MaxDiscountAmount { get; set; }

    public int UsageLimit { get; set; }
    public int UsedCount { get; set; }

    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
