using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Features.Orders.Dtos;
using PrintHub.Domain.Entities;

namespace PrintHub.Api.Controllers.OData;

/// <summary>
/// OData endpoint for orders (GET /odata/Orders). Demonstrates $filter, $orderby,
/// $top, $skip, $select and $expand=Items. A mandatory ownership filter is applied
/// server-side BEFORE the client's query options compose on top: a caller sees only
/// their own orders (as customer) or their shops' orders (as owner/staff), so a
/// crafted query can never reveal another party's orders.
/// </summary>
[Authorize]
public class OrdersController : ODataController
{
    private readonly IReadRepository<Order> _orders;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public OrdersController(IReadRepository<Order> orders, ICurrentUser currentUser, IMapper mapper)
    {
        _orders = orders;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    [EnableQuery(MaxTop = 100, PageSize = 50)]
    public IQueryable<OrderODataDto> Get()
    {
        var userId = _currentUser.UserId ?? 0;
        var shopIds = _currentUser.ShopIds.ToArray();
        var scoped = _orders.Query().Where(o => o.CustomerId == userId || shopIds.Contains(o.ShopId));
        return _mapper.ProjectTo<OrderODataDto>(scoped);
    }
}
