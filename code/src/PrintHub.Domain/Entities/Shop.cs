using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A print shop on the marketplace. Moves through the onboarding state machine
/// (Draft → PendingReview → Active / Rejected → Suspended) enforced in the
/// service layer.
/// </summary>
public class Shop : AuditableEntity, ISoftDelete
{
    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public string AddressLine { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PhoneNumber { get; set; }

    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }

    public ShopStatus Status { get; set; } = ShopStatus.Draft;

    /// <summary>Administrator's reason on rejection or suspension.</summary>
    public string? ReviewNote { get; set; }

    public double RatingAverage { get; set; }
    public int RatingCount { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public User Owner { get; set; } = null!;
    public ICollection<ShopStaff> Staff { get; set; } = new List<ShopStaff>();
    public ICollection<ShopService> Services { get; set; } = new List<ShopService>();
    public ICollection<Machine> Machines { get; set; } = new List<Machine>();
    public ICollection<Material> Materials { get; set; } = new List<Material>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
