using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A file in a customer's personal document library. The stored path is never
/// exposed to clients; files are served only through an endpoint that verifies
/// ownership or an active fulfilment relationship.
/// </summary>
public class DocumentFile : BaseEntity, ISoftDelete
{
    public int OwnerUserId { get; set; }

    public string FileName { get; set; } = null!;

    /// <summary>Server-side storage path. Never returned to clients.</summary>
    public string StoragePath { get; set; } = null!;

    public string ContentType { get; set; } = null!;
    public long FileSizeKb { get; set; }

    /// <summary>Page count declared by the customer at upload (see LI-6).</summary>
    public int? DeclaredPageCount { get; set; }

    /// <summary>Whether the customer accepted the intellectual property declaration.</summary>
    public bool RightsDeclared { get; set; }

    public DateTime UploadedAt { get; set; }
    public string? Checksum { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    /// <summary>Retention deadline after which the file is purged (data-protection policy).</summary>
    public DateTime? PurgeAfter { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public User Owner { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
