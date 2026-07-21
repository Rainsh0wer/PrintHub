using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Shops;

/// <summary>A shop's full rate card — entries with their service type and price rules.</summary>
public sealed class RateCardByShopSpecification : BaseSpecification<ShopService>
{
    public RateCardByShopSpecification(int shopId)
        : base(rc => rc.ShopId == shopId)
    {
        AddInclude(rc => rc.ServiceType);
        AddInclude(rc => rc.PriceRules);
        ApplyOrderBy(rc => rc.ServiceTypeId);
    }
}

/// <summary>A single rate card entry by id, tracked, with its price rules — for updates.</summary>
public sealed class ShopServiceByIdSpecification : BaseSpecification<ShopService>
{
    public ShopServiceByIdSpecification(int shopServiceId)
        : base(rc => rc.Id == shopServiceId)
    {
        AddInclude(rc => rc.PriceRules);
    }
}

/// <summary>A single rate card entry by id with its service type and price rules — for full DTO responses.</summary>
public sealed class ShopServiceWithTypeByIdSpecification : BaseSpecification<ShopService>
{
    public ShopServiceWithTypeByIdSpecification(int shopServiceId)
        : base(rc => rc.Id == shopServiceId)
    {
        AddInclude(rc => rc.ServiceType);
        AddInclude(rc => rc.PriceRules);
    }
}

/// <summary>Whether a shop already offers a given service type (uniqueness check).</summary>
public sealed class ShopServiceByShopAndTypeSpecification : BaseSpecification<ShopService>
{
    public ShopServiceByShopAndTypeSpecification(int shopId, int serviceTypeId)
        : base(rc => rc.ShopId == shopId && rc.ServiceTypeId == serviceTypeId)
    {
    }
}
