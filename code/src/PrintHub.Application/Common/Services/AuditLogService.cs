using System.Text.Json;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Common.Services;

/// <summary>
/// Default audit logger. Shares the request's unit of work (same scoped instance
/// as the calling service), so the audit row is persisted by the caller's
/// SaveChanges within the same transaction.
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;

    public AuditLogService(IUnitOfWork uow, ICurrentUser currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task AddAsync(
        string action, string entityName, string? entityId = null,
        object? oldValue = null, object? newValue = null, CancellationToken ct = default)
    {
        var log = new AuditLog
        {
            ActorUserId = _currentUser.UserId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue),
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<AuditLog>().AddAsync(log, ct);
    }
}
