using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Orders;

/// <summary>How many non-cancelled orders a customer has placed with a given voucher (per-user limit).</summary>
public sealed class OrdersByCustomerAndVoucherCountSpecification : BaseSpecification<Order>
{
    public OrdersByCustomerAndVoucherCountSpecification(int customerId, int voucherId)
        : base(o => o.CustomerId == customerId
                    && o.VoucherId == voucherId
                    && o.Status != OrderStatus.Cancelled
                    && o.Status != OrderStatus.Declined)
    {
    }
}
