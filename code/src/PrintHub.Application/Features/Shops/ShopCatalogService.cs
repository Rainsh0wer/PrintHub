using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Read-only shop discovery. Search and detail queries are expressed as
/// specifications; this service only orchestrates and maps to DTOs.
/// </summary>
public class ShopCatalogService : IShopCatalogService
{
    private readonly IRepository<Shop> _shops;
    private readonly IMapper _mapper;

    public ShopCatalogService(IRepository<Shop> shops, IMapper mapper)
    {
        _shops = shops;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ShopSummaryDto>>> SearchAsync(ShopSearchRequest request, CancellationToken ct = default)
    {
        var page = new PageRequest { PageNumber = request.PageNumber, PageSize = request.PageSize };
        var spec = new ShopSearchSpecification(request, page);

        var total = await _shops.CountAsync(spec, ct);
        var shops = await _shops.ListAsync(spec, ct);
        var items = _mapper.Map<IReadOnlyList<ShopSummaryDto>>(shops);

        return Result.Success(new PagedResult<ShopSummaryDto>(items, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<ShopDetailDto>> GetDetailAsync(int shopId, CancellationToken ct = default)
    {
        var shop = await _shops.FirstOrDefaultAsync(new ShopDetailByIdSpecification(shopId), ct);
        if (shop is null)
            return Result<ShopDetailDto>.NotFound("This shop could not be found.");

        return Result.Success(_mapper.Map<ShopDetailDto>(shop));
    }
}
