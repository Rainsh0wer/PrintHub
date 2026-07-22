namespace PrintHub.Application.Features.Reports.Dtos;

/// <summary>
/// A shop's revenue summary over an optional date window (UC-30). Plain settable
/// class with a parameterless constructor so it serialises to XML and CSV as well
/// as JSON (content negotiation). Kept flat for the same reason.
/// </summary>
public class ShopRevenueReportDto
{
    public int ShopId { get; set; }
    public string ShopName { get; set; } = "";
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public int CompletedOrders { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal CommissionTotal { get; set; }
    public decimal NetRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
}

/// <summary>Platform-wide transaction/revenue summary (UC-42), flat for content negotiation.</summary>
public class PlatformReportDto
{
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public int TotalShops { get; set; }
    public int ActiveShops { get; set; }
    public int CompletedOrders { get; set; }
    public decimal Gmv { get; set; }
    public decimal CommissionEarned { get; set; }
    public string? TopShopName { get; set; }
    public decimal TopShopRevenue { get; set; }
}
