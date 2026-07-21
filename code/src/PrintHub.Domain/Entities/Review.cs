using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A customer's rating of a shop for a completed order. At most one review may
/// exist per order (unique on OrderId); only a customer who transacted may review.
/// </summary>
public class Review : AuditableEntity
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int ShopId { get; set; }

    public int Rating { get; set; }
    public string? Comment { get; set; }

    public string? ShopReply { get; set; }
    public DateTime? RepliedAt { get; set; }
    public int? RepliedByUserId { get; set; }
    /// <summary>Moderation flag; a hidden review does not count toward the shop rating display.</summary>
    public bool IsVisible { get; set; } = true;

    // Navigation
    public Order Order { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
}
