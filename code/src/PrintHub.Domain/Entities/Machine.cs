using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A production machine belonging to a shop. Its status affects quote
/// eligibility: a shop with no non-offline machine for a service type cannot be
/// quoted for it.
/// </summary>
public class Machine : AuditableEntity, ISoftDelete
{
    public int ShopId { get; set; }

    public string Name { get; set; } = null!;
    public MachineType MachineType { get; set; }
    public ServiceGroup ServiceGroup { get; set; }
    public MachineStatus Status { get; set; } = MachineStatus.Idle;

    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? LastMaintenanceAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation
    public Shop Shop { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
