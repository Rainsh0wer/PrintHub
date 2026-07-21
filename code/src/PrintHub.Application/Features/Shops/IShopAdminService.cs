using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Administrator governance of shops: application review (UC-36) and
/// suspend/reinstate (UC-37). Drives the shop onboarding state machine.
/// </summary>
public interface IShopAdminService
{
    Task<Result<IReadOnlyList<ShopAdminListItemDto>>> ListPendingApplicationsAsync(CancellationToken ct = default);
    Task<Result> ApproveAsync(int shopId, CancellationToken ct = default);
    Task<Result> RejectAsync(int shopId, string reason, CancellationToken ct = default);
    Task<Result> SuspendAsync(int shopId, string reason, CancellationToken ct = default);
    Task<Result> ReinstateAsync(int shopId, CancellationToken ct = default);
}
