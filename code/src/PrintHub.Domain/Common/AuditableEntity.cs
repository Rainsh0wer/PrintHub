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

    /// <summary>User id who created the record (stamped centrally; null for system/seed).</summary>
    public int? CreatedBy { get; set; }

    /// <summary>User id who last updated the record.</summary>
    public int? UpdatedBy { get; set; }
}
