using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Reviews;

/// <summary>Whether a given order already has a review (one-per-order uniqueness).</summary>
public sealed class ReviewByOrderSpecification : BaseSpecification<Review>
{
    public ReviewByOrderSpecification(int orderId)
        : base(r => r.OrderId == orderId)
    {
    }
}

/// <summary>A review by id with its customer — for a complete response after create.</summary>
public sealed class ReviewWithCustomerByIdSpecification : BaseSpecification<Review>
{
    public ReviewWithCustomerByIdSpecification(int reviewId)
        : base(r => r.Id == reviewId)
    {
        AddInclude(r => r.Customer);
    }
}

/// <summary>A shop's public (visible) reviews, newest first, with the customer. Paged.</summary>
public sealed class VisibleReviewsByShopSpecification : BaseSpecification<Review>
{
    public VisibleReviewsByShopSpecification(int shopId, int skip, int take)
        : base(r => r.ShopId == shopId && r.IsVisible)
    {
        AddInclude(r => r.Customer);
        ApplyOrderByDescending(r => r.CreatedAt);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a shop's visible reviews — the paging companion.</summary>
public sealed class VisibleReviewsByShopCountSpecification : BaseSpecification<Review>
{
    public VisibleReviewsByShopCountSpecification(int shopId)
        : base(r => r.ShopId == shopId && r.IsVisible)
    {
    }
}
