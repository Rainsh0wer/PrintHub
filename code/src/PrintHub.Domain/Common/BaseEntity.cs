namespace PrintHub.Domain.Common;

/// <summary>
/// Base type for every persisted entity. Uses an int identity primary key,
/// matching the "auto-increment primary key" convention in the data dictionary.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
}
