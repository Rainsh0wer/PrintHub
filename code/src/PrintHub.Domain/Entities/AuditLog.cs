using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// An append-only record of an administrative or security-relevant action,
/// capturing before/after state for accountability.
/// </summary>
public class AuditLog : BaseEntity
{
    public int? ActorUserId { get; set; }

    /// <summary>Action performed, e.g. ApproveShop, LockUser, UpdateCommission.</summary>
    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;
    public string? EntityId { get; set; }

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User? ActorUser { get; set; }
}
