using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Favourites;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Favourites;

/// <summary>
/// Add/remove/list a customer's favourite shops. Add is idempotent (BR-25) and
/// the shop must exist and be active.
/// </summary>
public class FavouriteService : IFavouriteService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public FavouriteService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ShopSummaryDto>>> ListAsync(int customerId, CancellationToken ct = default)
    {
        var favourites = await _uow.Repository<Favourite>()
            .ListAsync(new FavouritesByCustomerSpecification(customerId), ct);

        var shops = favourites.Select(f => f.Shop);
        var items = _mapper.Map<IReadOnlyList<ShopSummaryDto>>(shops);
        return Result.Success(items);
    }

    public async Task<Result> AddAsync(int customerId, int shopId, CancellationToken ct = default)
    {
        // Shop must exist and be active.
        var shopExists = await _uow.Repository<Shop>().AnyAsync(new ActiveShopByIdSpecification(shopId), ct);
        if (!shopExists)
            return Result.NotFound("This shop could not be found.");

        var favourites = _uow.Repository<Favourite>();

        // BR-25: at most one favourite per (customer, shop) — treat a repeat as success.
        var already = await favourites.AnyAsync(new FavouriteByCustomerAndShopSpecification(customerId, shopId), ct);
        if (already)
            return Result.Success();

        await favourites.AddAsync(new Favourite
        {
            CustomerId = customerId,
            ShopId = shopId,
            CreatedAt = DateTime.UtcNow
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(int customerId, int shopId, CancellationToken ct = default)
    {
        var favourites = _uow.Repository<Favourite>();
        var existing = await favourites.FirstOrDefaultAsync(
            new FavouriteByCustomerAndShopSpecification(customerId, shopId), ct);

        if (existing is not null)
        {
            favourites.Remove(existing);
            await _uow.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
