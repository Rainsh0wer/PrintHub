using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// One configured line of an order: a file (or non-file service) with its
/// service-type-specific options. Effective unit price and line total are
/// snapshotted when the order is placed.
/// </summary>
public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ServiceTypeId { get; set; }
    public int? DocumentFileId { get; set; }

    public int Quantity { get; set; } = 1;
    public int? PageCount { get; set; }

    // Document / finishing options
    public string? PaperType { get; set; }
    public ColorMode? ColorMode { get; set; }
    public Sides? Sides { get; set; }
    public string? BindingType { get; set; }

    // Fabrication options
    public string? MaterialName { get; set; }
    public string? QualityProfile { get; set; }
    public decimal? EstimatedGrams { get; set; }

    /// <summary>Effective unit price after pricing rules, snapshotted.</summary>
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public string? ItemNote { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
    public ServiceType ServiceType { get; set; } = null!;
    public DocumentFile? DocumentFile { get; set; }
}
