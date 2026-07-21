using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.ServiceTypes;

/// <summary>An active service type by id (validating a shop can offer it).</summary>
public sealed class ActiveServiceTypeByIdSpecification : BaseSpecification<ServiceType>
{
    public ActiveServiceTypeByIdSpecification(int serviceTypeId)
        : base(t => t.Id == serviceTypeId && t.IsActive)
    {
    }
}
