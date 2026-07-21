using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A platform-wide service definition (e.g. A4 B/W printing, spiral binding,
/// FDM 3D print). Shops build their rate cards by selecting from this catalogue.
/// The <see cref="PricingModel"/> selects the pricing strategy applied at quote
/// time and cannot change once orders exist for the service.
/// </summary>
public class ServiceType : AuditableEntity, ISoftDelete
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public ServiceGroup ServiceGroup { get; set; }
    public PricingModel PricingModel { get; set; }

    /// <summary>Unit in which quantity is expressed: page, unit, gram.</summary>
    public string UnitOfMeasure { get; set; } = null!;

    public bool RequiresFile { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    // Navigation
    public ICollection<ShopService> ShopServices { get; set; } = new List<ShopService>();
}
