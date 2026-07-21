namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Records an administrative or security-relevant action. The entry is added to
/// the current unit of work (not saved), so it commits atomically with the action
/// that produced it. Actor comes from <see cref="ICurrentUser"/>.
/// </summary>
public interface IAuditLogService
{
    Task AddAsync(
        string action,
        string entityName,
        string? entityId = null,
        object? oldValue = null,
        object? newValue = null,
        CancellationToken ct = default);
}
