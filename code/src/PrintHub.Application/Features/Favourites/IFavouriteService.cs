using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Favourites;

/// <summary>Managing a customer's favourite shops (UC-11).</summary>
public interface IFavouriteService
{
    Task<Result<IReadOnlyList<ShopSummaryDto>>> ListAsync(int customerId, CancellationToken ct = default);
    Task<Result> AddAsync(int customerId, int shopId, CancellationToken ct = default);
    Task<Result> RemoveAsync(int customerId, int shopId, CancellationToken ct = default);
}
