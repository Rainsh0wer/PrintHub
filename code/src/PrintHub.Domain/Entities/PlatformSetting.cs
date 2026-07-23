using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>Singleton platform configuration (one row). Currently the commission rate.</summary>
public class PlatformSetting : BaseEntity
{
    public decimal CommissionRate { get; set; }
    public DateTime UpdatedAt { get; set; }
}
