using PrintHub.Application.Common.Interfaces;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Common;

/// <summary>
/// Scoped-authorization helpers over the current user's shop-membership claims.
/// This is the second authorization dimension beyond role: not just "what role"
/// but "which shop". Services call these before touching shop-scoped data.
/// </summary>
public static class ShopAccess
{
    /// <summary>True if the caller is the owner of the given shop (owner-only operations).</summary>
    public static bool CanManageShop(this ICurrentUser user, int shopId)
        => user.Role == UserRole.ShopOwner && user.BelongsToShop(shopId);

    /// <summary>True if the caller is the owner or active staff of the given shop (operational tasks).</summary>
    public static bool CanOperateShop(this ICurrentUser user, int shopId)
        => (user.Role == UserRole.ShopOwner || user.Role == UserRole.ShopStaff) && user.BelongsToShop(shopId);
}
