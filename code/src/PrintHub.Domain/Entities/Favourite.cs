using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A customer's saved shop. Junction between <see cref="User"/> and
/// <see cref="Shop"/> with a composite unique constraint on (CustomerId, ShopId).
/// </summary>
public class Favourite : BaseEntity
{
    public int CustomerId { get; set; }
    public int ShopId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User Customer { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
}
