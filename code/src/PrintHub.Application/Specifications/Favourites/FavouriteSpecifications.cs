using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.Favourites;

/// <summary>A specific customer's favourite record for a specific shop.</summary>
public sealed class FavouriteByCustomerAndShopSpecification : BaseSpecification<Favourite>
{
    public FavouriteByCustomerAndShopSpecification(int customerId, int shopId)
        : base(f => f.CustomerId == customerId && f.ShopId == shopId)
    {
    }
}

/// <summary>All of a customer's favourite shops, newest first, with the shop loaded.</summary>
public sealed class FavouritesByCustomerSpecification : BaseSpecification<Favourite>
{
    public FavouritesByCustomerSpecification(int customerId)
        : base(f => f.CustomerId == customerId)
    {
        // Include the shop's rate card so the summary can show service groups and price.
        AddInclude("Shop.Services.ServiceType");
        ApplyOrderByDescending(f => f.CreatedAt);
    }
}
