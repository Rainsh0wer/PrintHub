using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;

namespace PrintHub.Application.Features.Shops;

/// <summary>Owner management of shop staff (UC-29). Owner-scoped.</summary>
public interface IShopStaffService
{
    Task<Result<IReadOnlyList<StaffDto>>> ListAsync(int shopId, CancellationToken ct = default);
    Task<Result> GrantAsync(int shopId, GrantStaffRequest request, CancellationToken ct = default);
    Task<Result> RevokeAsync(int shopId, int staffId, CancellationToken ct = default);
}
