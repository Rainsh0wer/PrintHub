using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A dispute raised against a completed order. Flows Open → ShopResponded →
/// Resolved, or escalates to an administrator whose ruling is final. At most one
/// open complaint may exist per order.
/// </summary>
public class Complaint : AuditableEntity
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int ShopId { get; set; }

    public ComplaintReason Reason { get; set; }
    public string Description { get; set; } = null!;

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;

    public ComplaintResolution? ProposedResolution { get; set; }
    public string? ShopResponse { get; set; }
    public string? AdminRuling { get; set; }

    public decimal? RefundAmount { get; set; }

    /// <summary>Linked zero-charge reprint order; null unless resolution is Reprint.</summary>
    public int? ReplacementOrderId { get; set; }

    public int? ResolvedBy { get; set; }
    /// <summary>Comma/JSON list of evidence attachment URLs supplied by the customer.</summary>
    public string? AttachmentUrls { get; set; }

    public DateTime? RespondedAt { get; set; }
    public DateTime? EscalatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
    public Order? ReplacementOrder { get; set; }
}
