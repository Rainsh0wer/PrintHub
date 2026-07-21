using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>Owner management of a shop's storefront profile (UC-26).</summary>
public interface IShopProfileService
{
    Task<Result<ShopApplicationDto>> UpdateProfileAsync(int shopId, ShopApplicationRequest request, CancellationToken ct = default);
}
