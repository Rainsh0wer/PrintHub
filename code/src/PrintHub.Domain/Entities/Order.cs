using PrintHub.Domain.Common;
using PrintHub.Domain.Enums;

namespace PrintHub.Domain.Entities;

/// <summary>
/// The central transaction aggregate. Progresses through the order lifecycle
/// state machine; every transition is recorded in <see cref="OrderStatusHistory"/>
/// with actor, timestamp, and reason.
/// </summary>
public class Order : AuditableEntity
{
    /// <summary>Human-readable code used at the counter, e.g. PH-260720-0041.</summary>
    public string OrderCode { get; set; } = null!;

    public int CustomerId { get; set; }
    public int ShopId { get; set; }
    public int? QuoteId { get; set; }
    public int? MachineId { get; set; }
    public int? VoucherId { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Draft;

    public FulfilmentMethod FulfilmentMethod { get; set; } = FulfilmentMethod.Pickup;
    public DateTime? PickupSlotStart { get; set; }
    public DateTime? PickupSlotEnd { get; set; }
    public string? DeliveryAddress { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }

    /// <summary>Commission rate snapshotted at completion.</summary>
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }

    public int ProgressPercent { get; set; }

    public string? CustomerNote { get; set; }
    public DeclineReason? DeclineReason { get; set; }

    public DateTime? PlacedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation
    public User Customer { get; set; } = null!;
    public Shop Shop { get; set; } = null!;
    public Quote? Quote { get; set; }
    public Machine? Machine { get; set; }
    public Voucher? Voucher { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public Review? Review { get; set; }
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
