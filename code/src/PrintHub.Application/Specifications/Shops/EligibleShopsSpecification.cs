using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Shops;

/// <summary>
/// Active shops loaded with their rate card and machines, for quote eligibility.
/// Final eligibility (offers every requested service, has a non-offline machine
/// for each service group) is decided in the service against the loaded graph.
/// </summary>
public sealed class ActiveShopsWithRateCardSpecification : BaseSpecification<Shop>
{
    public ActiveShopsWithRateCardSpecification(int? shopId)
        : base(s => s.Status == ShopStatus.Active && (shopId == null || s.Id == shopId))
    {
        AddInclude("Services.ServiceType");
        AddInclude("Services.PriceRules");
        AddInclude(s => s.Machines);
        AsReadOnly();
    }
}
