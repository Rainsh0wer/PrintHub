using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Reviews.Dtos;

namespace PrintHub.Application.Features.Reviews;

/// <summary>
/// Reviews (UC-23). A customer may rate a completed order exactly once; posting a
/// review updates the shop's aggregate rating. Shop reviews are public.
/// </summary>
public interface IReviewService
{
    /// <summary>UC-23 — rate a completed order (one per order); recomputes the shop rating.</summary>
    Task<Result<ReviewItemDto>> CreateAsync(int customerId, int orderId, CreateReviewRequest request, CancellationToken ct = default);

    /// <summary>UC-23 — a shop's public reviews, newest first, paged.</summary>
    Task<Result<PagedResult<ReviewItemDto>>> ListForShopAsync(int shopId, PageRequest page, CancellationToken ct = default);
}
