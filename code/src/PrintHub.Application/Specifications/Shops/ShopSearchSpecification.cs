using PrintHub.Application.Common.Models;
using PrintHub.Application.Common.Specifications;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Specifications.Shops;

/// <summary>
/// The shop directory query (UC-09). Encapsulates the mandatory Active filter
/// plus optional keyword/district/service-group/rating filters, ordering, and
/// paging — the whole search expressed as one reusable specification.
/// </summary>
public sealed class ShopSearchSpecification : BaseSpecification<Shop>
{
    public ShopSearchSpecification(ShopSearchRequest request, PageRequest page)
    {
        Where(s =>
            s.Status == ShopStatus.Active
            && (string.IsNullOrEmpty(request.Keyword)
                || s.Name.Contains(request.Keyword)
                || (s.Description != null && s.Description.Contains(request.Keyword)))
            && (string.IsNullOrEmpty(request.District) || s.District == request.District)
            && (request.MinRating == null || s.RatingAverage >= request.MinRating)
            && (request.ServiceGroup == null
                || s.Services.Any(ss => ss.IsActive && ss.ServiceType.ServiceGroup == request.ServiceGroup)));

        // Rate card is needed to project service groups and the indicative price.
        AddInclude("Services.ServiceType");

        ApplySort(request);
        ApplyPaging(page.Skip, page.Take);
    }

    private void ApplySort(ShopSearchRequest request)
    {
        switch (request.SortBy)
        {
            case ShopSortBy.Name:
                if (request.Descending) ApplyOrderByDescending(s => s.Name);
                else ApplyOrderBy(s => s.Name);
                break;
            case ShopSortBy.Rating:
            default:
                if (request.Descending) ApplyOrderByDescending(s => s.RatingAverage);
                else ApplyOrderBy(s => s.RatingAverage);
                break;
        }
    }
}

/// <summary>A single active shop by id, no includes — for existence/status checks.</summary>
public sealed class ActiveShopByIdSpecification : BaseSpecification<Shop>
{
    public ActiveShopByIdSpecification(int shopId)
        : base(s => s.Id == shopId && s.Status == ShopStatus.Active)
    {
    }
}

/// <summary>Loads a single shop with the data needed for its detail page (UC-10).</summary>
public sealed class ShopDetailByIdSpecification : BaseSpecification<Shop>
{
    public ShopDetailByIdSpecification(int shopId)
        : base(s => s.Id == shopId && s.Status == ShopStatus.Active)
    {
        AddInclude("Services.ServiceType");
        AddInclude(s => s.Machines);
        AddInclude("Reviews.Customer");
    }
}
