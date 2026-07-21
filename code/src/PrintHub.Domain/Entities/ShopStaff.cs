using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// Junction between <see cref="User"/> and <see cref="Shop"/> that carries its
/// own attributes — a many-to-many-with-attributes relationship. This record is
/// what makes scoped authorization possible: it defines which shop a staff
/// member may operate on.
/// </summary>
public class ShopStaff : BaseEntity
{
    public int ShopId { get; set; }
    public int UserId { get; set; }

    public string? Position { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Shop Shop { get; set; } = null!;
    public User User { get; set; } = null!;
}
