using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// A stock item held by a shop (paper, filament, sheet, consumable). Tracked as
/// an availability indicator with a low-stock alert rather than a reserved
/// quantity ledger.
/// </summary>
public class Material : AuditableEntity, ISoftDelete
{
    public int ShopId { get; set; }

    public string Name { get; set; } = null!;
    public MaterialType MaterialType { get; set; }

    /// <summary>Stock unit: sheet, gram, piece.</summary>
    public string Unit { get; set; } = null!;

    public decimal StockQuantity { get; set; }
    public decimal LowStockThreshold { get; set; }

    /// <summary>Shop's cost per unit, used in margin reporting. Never exposed publicly.</summary>
    public decimal UnitCost { get; set; }

    public string? Sku { get; set; }
    public string? Color { get; set; }
    public decimal? ReorderQuantity { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsLowStock => StockQuantity <= LowStockThreshold;

    // Navigation
    public Shop Shop { get; set; } = null!;
}
