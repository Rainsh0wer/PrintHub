using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>Customer-side shop onboarding (UC-25) and the owner's shop list.</summary>
public interface IShopOnboardingService
{
    Task<Result<ShopApplicationDto>> ApplyAsync(int userId, ShopApplicationRequest request, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ShopApplicationDto>>> GetMyShopsAsync(int userId, CancellationToken ct = default);
}
