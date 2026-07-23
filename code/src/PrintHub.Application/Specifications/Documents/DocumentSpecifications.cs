using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Documents;

public sealed class DocumentsByOwnerSpecification : BaseSpecification<DocumentFile>
{
    public DocumentsByOwnerSpecification(int ownerId, int skip, int take)
        : base(d => d.OwnerUserId == ownerId && !d.IsDeleted)
    {
        ApplyOrderByDescending(d => d.UploadedAt);
        ApplyPaging(skip, take);
    }
}

public sealed class DocumentsByOwnerCountSpecification : BaseSpecification<DocumentFile>
{
    public DocumentsByOwnerCountSpecification(int ownerId)
        : base(d => d.OwnerUserId == ownerId && !d.IsDeleted)
    {
    }
}

/// <summary>Order lines referencing a document on an order that is still in progress
/// (blocks deletion of a document that is on an active order).</summary>
public sealed class ActiveOrderItemsByDocumentSpecification : BaseSpecification<OrderItem>
{
    public ActiveOrderItemsByDocumentSpecification(int documentId)
        : base(oi => oi.DocumentFileId == documentId
                     && oi.Order.Status != OrderStatus.Completed
                     && oi.Order.Status != OrderStatus.Cancelled
                     && oi.Order.Status != OrderStatus.Declined
                     && oi.Order.Status != OrderStatus.Expired)
    {
    }
}
