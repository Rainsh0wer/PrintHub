using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// One transition in an order's lifecycle. Records are append-only and never
/// modified or deleted, which is what makes them usable as evidence during
/// complaint resolution.
/// </summary>
public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }

    public OrderStatus? FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }

    /// <summary>User who performed the transition; null for system transitions.</summary>
    public int? ActorUserId { get; set; }
    public UserRole? ActorRole { get; set; }

    public string? Reason { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public User? ActorUser { get; set; }
}
