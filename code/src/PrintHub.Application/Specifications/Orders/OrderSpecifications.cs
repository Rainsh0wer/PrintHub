using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Orders;

/// <summary>An order with everything the detail view needs: shop, items, and full history.</summary>
public sealed class OrderWithDetailsByIdSpecification : BaseSpecification<Order>
{
    public OrderWithDetailsByIdSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.Shop);
        AddInclude(o => o.Items);
        AddInclude(o => o.StatusHistory);
    }
}

/// <summary>A single order by id, tracked (no includes) — for lifecycle transitions.</summary>
public sealed class OrderByIdSpecification : BaseSpecification<Order>
{
    public OrderByIdSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
    }
}

/// <summary>A customer's order history, newest first, with the shop for display. Paged.</summary>
public sealed class OrdersByCustomerSpecification : BaseSpecification<Order>
{
    public OrdersByCustomerSpecification(int customerId, int skip, int take, OrderStatus? status = null)
        : base(o => o.CustomerId == customerId && (status == null || o.Status == status))
    {
        AddInclude(o => o.Shop);
        ApplyOrderByDescending(o => o.PlacedAt!);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a customer's orders (optionally filtered by status) — the paging companion.</summary>
public sealed class OrdersByCustomerCountSpecification : BaseSpecification<Order>
{
    public OrdersByCustomerCountSpecification(int customerId, OrderStatus? status = null)
        : base(o => o.CustomerId == customerId && (status == null || o.Status == status))
    {
    }
}

/// <summary>A shop's order queue, newest first, with the customer for display. Paged.</summary>
public sealed class OrdersByShopSpecification : BaseSpecification<Order>
{
    public OrdersByShopSpecification(int shopId, int skip, int take, OrderStatus? status = null)
        : base(o => o.ShopId == shopId && (status == null || o.Status == status))
    {
        AddInclude(o => o.Shop);
        ApplyOrderByDescending(o => o.PlacedAt!);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a shop's orders (optionally filtered by status) — the paging companion.</summary>
public sealed class OrdersByShopCountSpecification : BaseSpecification<Order>
{
    public OrdersByShopCountSpecification(int shopId, OrderStatus? status = null)
        : base(o => o.ShopId == shopId && (status == null || o.Status == status))
    {
    }
}
