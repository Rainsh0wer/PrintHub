using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Reports;

/// <summary>Completed orders for one shop within an optional date window (shop revenue report).</summary>
public sealed class CompletedOrdersByShopSpecification : BaseSpecification<Order>
{
    public CompletedOrdersByShopSpecification(int shopId, DateTime? from, DateTime? to)
        : base(o => o.ShopId == shopId
                    && o.Status == OrderStatus.Completed
                    && (from == null || o.CompletedAt >= from)
                    && (to == null || o.CompletedAt <= to))
    {
    }
}

/// <summary>Completed orders platform-wide within an optional date window, with the shop for ranking.</summary>
public sealed class CompletedOrdersInRangeSpecification : BaseSpecification<Order>
{
    public CompletedOrdersInRangeSpecification(DateTime? from, DateTime? to)
        : base(o => o.Status == OrderStatus.Completed
                    && (from == null || o.CompletedAt >= from)
                    && (to == null || o.CompletedAt <= to))
    {
        AddInclude(o => o.Shop);
    }
}
