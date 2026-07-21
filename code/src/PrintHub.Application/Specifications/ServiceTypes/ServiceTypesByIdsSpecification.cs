using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.ServiceTypes;

/// <summary>Load a set of service types by id (to resolve pricing model + group).</summary>
public sealed class ServiceTypesByIdsSpecification : BaseSpecification<ServiceType>
{
    public ServiceTypesByIdsSpecification(IReadOnlyCollection<int> ids)
        : base(t => ids.Contains(t.Id))
    {
        AsReadOnly();
    }
}
