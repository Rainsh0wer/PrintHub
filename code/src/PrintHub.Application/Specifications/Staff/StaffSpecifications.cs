using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Staff;

/// <summary>All staff memberships of a shop, with the user, newest first.</summary>
public sealed class StaffByShopSpecification : BaseSpecification<ShopStaff>
{
    public StaffByShopSpecification(int shopId)
        : base(s => s.ShopId == shopId)
    {
        AddInclude(s => s.User);
        ApplyOrderByDescending(s => s.JoinedAt);
    }
}

/// <summary>A membership for a specific (shop, user) pair — for grant reactivation checks.</summary>
public sealed class StaffMembershipSpecification : BaseSpecification<ShopStaff>
{
    public StaffMembershipSpecification(int shopId, int userId)
        : base(s => s.ShopId == shopId && s.UserId == userId)
    {
    }
}

/// <summary>A membership by its id — for revoke.</summary>
public sealed class StaffByIdSpecification : BaseSpecification<ShopStaff>
{
    public StaffByIdSpecification(int staffId)
        : base(s => s.Id == staffId)
    {
    }
}
