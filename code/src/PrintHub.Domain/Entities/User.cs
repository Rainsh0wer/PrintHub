using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A platform account. A single user may hold the Customer role and also own a
/// shop, so role and shop membership are modelled separately rather than as one
/// mutually exclusive field.
/// </summary>
public class User : AuditableEntity, ISoftDelete
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }

    /// <summary>BCrypt hash with per-user salt. Never returned by any API.</summary>
    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; } = UserRole.Customer;
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>Current wallet balance in VND. Changed only through WalletTransaction.</summary>
    public decimal WalletBalance { get; set; }

    public string? DefaultAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Shop> OwnedShops { get; set; } = new List<Shop>();
    public ICollection<ShopStaff> StaffMemberships { get; set; } = new List<ShopStaff>();
    public ICollection<DocumentFile> Documents { get; set; } = new List<DocumentFile>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
