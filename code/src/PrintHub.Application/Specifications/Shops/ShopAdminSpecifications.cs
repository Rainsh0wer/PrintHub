using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Shops;

/// <summary>A shop by id in ANY status, tracked — for admin actions and owner edits.</summary>
public sealed class ShopByIdSpecification : BaseSpecification<Shop>
{
    public ShopByIdSpecification(int shopId)
        : base(s => s.Id == shopId)
    {
    }
}

/// <summary>All shops owned by a user, newest first — the owner's "my shops" list.</summary>
public sealed class OwnedShopsSpecification : BaseSpecification<Shop>
{
    public OwnedShopsSpecification(int ownerId)
        : base(s => s.OwnerId == ownerId)
    {
        ApplyOrderByDescending(s => s.CreatedAt);
    }
}

/// <summary>Whether a user already has a shop that blocks a new application (BR-66).</summary>
public sealed class BlockingShopByOwnerSpecification : BaseSpecification<Shop>
{
    public BlockingShopByOwnerSpecification(int ownerId)
        : base(s => s.OwnerId == ownerId
                    && (s.Status == ShopStatus.PendingReview || s.Status == ShopStatus.Active))
    {
    }
}

/// <summary>Shops in a given status, with owner loaded — for admin lists.</summary>
public sealed class ShopsByStatusSpecification : BaseSpecification<Shop>
{
    public ShopsByStatusSpecification(ShopStatus status)
        : base(s => s.Status == status)
    {
        AddInclude(s => s.Owner);
        ApplyOrderBy(s => s.CreatedAt);
    }
}
