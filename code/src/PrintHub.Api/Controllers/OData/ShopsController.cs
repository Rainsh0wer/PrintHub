using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Api.Controllers.OData;

/// <summary>
/// OData endpoint for the shop directory (GET /odata/Shops). Demonstrates
/// $filter, $orderby, $top, $skip, $select. A mandatory Active filter is applied
/// server-side (BR-21/23) BEFORE the client's query options compose on top, so a
/// crafted query can never widen the result set beyond active shops.
/// </summary>
public class ShopsController : ODataController
{
    private readonly IReadRepository<Shop> _shops;
    private readonly IMapper _mapper;

    public ShopsController(IReadRepository<Shop> shops, IMapper mapper)
    {
        _shops = shops;
        _mapper = mapper;
    }

    [EnableQuery(MaxTop = 100, PageSize = 50)]
    public IQueryable<ShopODataDto> Get()
        => _mapper.ProjectTo<ShopODataDto>(
            _shops.Query().Where(s => s.Status == ShopStatus.Active));
}
