namespace PrintHub.Domain.Common;

/// <summary>
/// Marks an entity as logically deletable. Records are flagged rather than
/// physically removed, and are excluded from queries by a global query filter
/// configured in the DbContext, so historical orders keep resolvable references.
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
