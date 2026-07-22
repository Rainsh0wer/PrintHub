using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Notifications.Dtos;
using PrintHub.Application.Specifications.Notifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Notifications;

/// <summary>
/// Notification centre backing UC-08 plus a create hook other services use. All
/// reads and mark-as-read operations are scoped to the caller by user id.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<NotificationDto>>> ListAsync(int userId, PageRequest page, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Notification>();
        var total = await repo.CountAsync(new NotificationsByUserCountSpecification(userId), ct);
        var items = await repo.ListAsync(new NotificationsByUserSpecification(userId, page.Skip, page.Take), ct);

        var mapped = _mapper.Map<IReadOnlyList<NotificationDto>>(items);
        return Result.Success(new PagedResult<NotificationDto>(mapped, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<int>> MarkReadAsync(int userId, IReadOnlyCollection<int>? ids, CancellationToken ct = default)
    {
        var repo = _uow.Repository<Notification>();
        var toMark = ids is { Count: > 0 }
            ? await repo.ListAsync(new NotificationsByIdsForUserSpecification(userId, ids), ct)
            : await repo.ListAsync(new UnreadNotificationsByUserSpecification(userId), ct);

        var now = DateTime.UtcNow;
        var count = 0;
        foreach (var n in toMark.Where(n => !n.IsRead))
        {
            n.IsRead = true;
            n.ReadAt = now;
            repo.Update(n);
            count++;
        }

        if (count > 0) await _uow.SaveChangesAsync(ct);
        return Result.Success(count);
    }

    public async Task CreateAsync(int userId, NotificationType type, string title, string content,
        int? relatedOrderId = null, string? linkUrl = null, CancellationToken ct = default)
    {
        await _uow.Repository<Notification>().AddAsync(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Content = content,
            RelatedOrderId = relatedOrderId,
            LinkUrl = linkUrl,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
