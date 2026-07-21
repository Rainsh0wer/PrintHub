namespace PrintHub.Domain.Common;

/// <summary>
/// Entity that carries creation and update timestamps. Both are stored in UTC
/// and are stamped centrally by the DbContext SaveChanges override, not by
/// callers, so the values are consistent across the whole system.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
