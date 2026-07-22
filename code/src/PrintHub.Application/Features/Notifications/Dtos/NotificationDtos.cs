namespace PrintHub.Application.Features.Notifications.Dtos;

public record NotificationDto(
    int Id,
    string Title,
    string Content,
    string Type,
    int? RelatedOrderId,
    bool IsRead,
    string? LinkUrl,
    DateTime CreatedAt,
    DateTime? ReadAt);

/// <summary>UC-08 — mark notifications read; empty/omitted Ids marks all unread ones.</summary>
public record MarkNotificationsReadRequest(int[]? Ids);
