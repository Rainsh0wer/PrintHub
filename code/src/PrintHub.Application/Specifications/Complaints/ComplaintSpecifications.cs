using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Complaints;

/// <summary>A non-terminal complaint already open for an order (one-active-per-order guard).</summary>
public sealed class ActiveComplaintByOrderSpecification : BaseSpecification<Complaint>
{
    public ActiveComplaintByOrderSpecification(int orderId)
        : base(c => c.OrderId == orderId
                    && (c.Status == ComplaintStatus.Open
                        || c.Status == ComplaintStatus.ShopResponded
                        || c.Status == ComplaintStatus.Escalated))
    {
    }
}

/// <summary>A complaint by id with its order (for OrderCode/total) — tracked, for transitions.</summary>
public sealed class ComplaintByIdSpecification : BaseSpecification<Complaint>
{
    public ComplaintByIdSpecification(int complaintId)
        : base(c => c.Id == complaintId)
    {
        AddInclude(c => c.Order);
    }
}

/// <summary>A complaint by id with order + shop — for a complete DTO response.</summary>
public sealed class ComplaintWithDetailsByIdSpecification : BaseSpecification<Complaint>
{
    public ComplaintWithDetailsByIdSpecification(int complaintId)
        : base(c => c.Id == complaintId)
    {
        AddInclude(c => c.Order);
        AddInclude(c => c.Shop);
    }
}

/// <summary>A customer's complaints, newest first, with order + shop. Paged.</summary>
public sealed class ComplaintsByCustomerSpecification : BaseSpecification<Complaint>
{
    public ComplaintsByCustomerSpecification(int customerId, int skip, int take)
        : base(c => c.CustomerId == customerId)
    {
        AddInclude(c => c.Order);
        AddInclude(c => c.Shop);
        ApplyOrderByDescending(c => c.CreatedAt);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of a customer's complaints — the paging companion.</summary>
public sealed class ComplaintsByCustomerCountSpecification : BaseSpecification<Complaint>
{
    public ComplaintsByCustomerCountSpecification(int customerId)
        : base(c => c.CustomerId == customerId)
    {
    }
}

/// <summary>Escalated complaints awaiting an administrator, oldest first. Paged.</summary>
public sealed class EscalatedComplaintsSpecification : BaseSpecification<Complaint>
{
    public EscalatedComplaintsSpecification(int skip, int take)
        : base(c => c.Status == ComplaintStatus.Escalated)
    {
        AddInclude(c => c.Order);
        AddInclude(c => c.Shop);
        ApplyOrderBy(c => c.EscalatedAt!);
        ApplyPaging(skip, take);
    }
}

/// <summary>Count of escalated complaints — the paging companion.</summary>
public sealed class EscalatedComplaintsCountSpecification : BaseSpecification<Complaint>
{
    public EscalatedComplaintsCountSpecification()
        : base(c => c.Status == ComplaintStatus.Escalated)
    {
    }
}
