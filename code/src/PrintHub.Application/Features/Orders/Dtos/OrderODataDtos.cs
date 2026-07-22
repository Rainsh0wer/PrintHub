using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Orders.Dtos;

/// <summary>
/// OData projection of an order. A mutable class with an Id key so OData can shape
/// ($select), query ($filter/$orderby), and expand ($expand=Items) it. Enum status
/// is kept as an enum so the whole projection translates to SQL.
/// </summary>
public class OrderODataDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public int CustomerId { get; set; }
    public int ShopId { get; set; }
    public string ShopName { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public FulfilmentMethod FulfilmentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime? PlacedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    /// <summary>Order lines — an expandable navigation ($expand=Items).</summary>
    public List<OrderItemODataDto> Items { get; set; } = new();
}

/// <summary>Entity type (key Id) used inside <see cref="OrderODataDto.Items"/> for $expand.</summary>
public class OrderItemODataDto
{
    public int Id { get; set; }
    public int ServiceTypeId { get; set; }
    public int Quantity { get; set; }
    public int? PageCount { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
