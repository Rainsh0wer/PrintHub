using PrintHub.Application.Features.Catalog.Dtos;
using PrintHub.Application.Features.Quotes.Dtos;

namespace PrintHub.Web.Models;

public class QuoteVm
{
    public List<ServiceTypeAdminDto> Services { get; set; } = new();
    public int ServiceTypeId { get; set; }
    public int Quantity { get; set; } = 1;
    public int? PageCount { get; set; } = 20;
    public decimal? EstimatedGrams { get; set; }
    public int ColorMode { get; set; }      // 0 = B&W, 1 = Colour
    public int Sides { get; set; }           // 0 = simplex, 1 = duplex
    public int? ShopId { get; set; }
    public int SortBy { get; set; }          // 0 price, 1 time, 2 rating
    public string? VoucherCode { get; set; }

    public List<QuoteComparisonDto>? Results { get; set; }
    public string? Error { get; set; }

    // quoteId -> discount amount, populated when a voucher code is entered.
    public Dictionary<int, decimal> VoucherDiscounts { get; set; } = new();
    public string? VoucherNote { get; set; }
}
