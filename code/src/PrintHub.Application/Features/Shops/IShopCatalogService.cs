using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>Public shop discovery use cases (UC-09, UC-10).</summary>
public interface IShopCatalogService
{
    Task<Result<PagedResult<ShopSummaryDto>>> SearchAsync(ShopSearchRequest request, CancellationToken ct = default);
    Task<Result<ShopDetailDto>> GetDetailAsync(int shopId, CancellationToken ct = default);
}
