using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// An in-app notification for a user, created by an event consumer rather than
/// synchronously within the originating operation.
/// </summary>
public class Notification : BaseEntity
{
    public int UserId { get; set; }

    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public NotificationType Type { get; set; }

    public int? RelatedOrderId { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    /// <summary>Deep link the client opens when the notification is tapped.</summary>
    public string? LinkUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
