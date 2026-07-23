using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.ServiceTypes;

public sealed class AllServiceTypesSpecification : BaseSpecification<ServiceType>
{
    public AllServiceTypesSpecification()
    {
        ApplyOrderBy(t => t.DisplayOrder);
    }
}

public sealed class ServiceTypeByCodeSpecification : BaseSpecification<ServiceType>
{
    public ServiceTypeByCodeSpecification(string code)
        : base(t => t.Code == code)
    {
    }
}
