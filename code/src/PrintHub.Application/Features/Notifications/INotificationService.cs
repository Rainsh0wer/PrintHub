using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Notifications.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Notifications;

/// <summary>
/// In-app notifications (UC-08). The read operations back the notification centre;
/// <see cref="CreateAsync"/> is called by other services to raise a notification
/// when something happens to a user's order/complaint/wallet.
/// </summary>
public interface INotificationService
{
    /// <summary>UC-08 — list the caller's notifications, newest first, paged.</summary>
    Task<Result<PagedResult<NotificationDto>>> ListAsync(int userId, PageRequest page, CancellationToken ct = default);

    /// <summary>UC-08 — mark notifications read (all unread when no ids given); returns the count marked.</summary>
    Task<Result<int>> MarkReadAsync(int userId, IReadOnlyCollection<int>? ids, CancellationToken ct = default);

    /// <summary>Raise a notification for a user (called internally by other use cases).</summary>
    Task CreateAsync(int userId, NotificationType type, string title, string content, int? relatedOrderId = null, string? linkUrl = null, CancellationToken ct = default);
}
