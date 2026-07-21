using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops.Dtos;

public enum ShopSortBy
{
    Rating = 0,
    Name = 1
}

/// <summary>Filter/sort/paging options for the shop directory (UC-09).</summary>
public class ShopSearchRequest
{
    public string? Keyword { get; set; }
    public ServiceGroup? ServiceGroup { get; set; }
    public string? District { get; set; }
    public double? MinRating { get; set; }
    public ShopSortBy SortBy { get; set; } = ShopSortBy.Rating;
    public bool Descending { get; set; } = true;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
