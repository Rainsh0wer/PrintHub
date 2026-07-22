using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Notifications;

/// <summary>A user's notifications, newest first. Paged.</summary>
public sealed class NotificationsByUserSpecification : BaseSpecification<Notification>
{
    public NotificationsByUserSpecification(int userId, int skip, int take)
        : base(n => n.UserId == userId)
    {
        ApplyOrderByDescending(n => n.CreatedAt);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a user's notifications — the paging companion.</summary>
public sealed class NotificationsByUserCountSpecification : BaseSpecification<Notification>
{
    public NotificationsByUserCountSpecification(int userId)
        : base(n => n.UserId == userId)
    {
    }
}

/// <summary>A user's unread notifications — for "mark all as read".</summary>
public sealed class UnreadNotificationsByUserSpecification : BaseSpecification<Notification>
{
    public UnreadNotificationsByUserSpecification(int userId)
        : base(n => n.UserId == userId && !n.IsRead)
    {
    }
}

/// <summary>A user's notifications matching the given ids — for "mark selected as read".</summary>
public sealed class NotificationsByIdsForUserSpecification : BaseSpecification<Notification>
{
    public NotificationsByIdsForUserSpecification(int userId, IReadOnlyCollection<int> ids)
        : base(n => n.UserId == userId && ids.Contains(n.Id))
    {
    }
}
