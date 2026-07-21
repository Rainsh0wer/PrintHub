using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Shops;

/// <summary>Shops owned by a given user (used to build shop-membership claims).</summary>
public sealed class ShopsByOwnerSpecification : BaseSpecification<Shop>
{
    public ShopsByOwnerSpecification(int ownerId)
        : base(s => s.OwnerId == ownerId)
    {
        AsReadOnly();
    }
}

/// <summary>Active staff memberships of a given user.</summary>
public sealed class ActiveStaffByUserSpecification : BaseSpecification<ShopStaff>
{
    public ActiveStaffByUserSpecification(int userId)
        : base(s => s.UserId == userId && s.IsActive)
    {
        AsReadOnly();
    }
}
