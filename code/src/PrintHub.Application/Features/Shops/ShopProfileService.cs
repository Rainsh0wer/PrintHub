using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Lets a shop owner edit their own storefront profile. Access is scoped: the
/// caller must be the owner of the shop being edited (UC-26, BR-70).
/// </summary>
public class ShopProfileService : IShopProfileService
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ShopProfileService(IUnitOfWork uow, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<ShopApplicationDto>> UpdateProfileAsync(int shopId, ShopApplicationRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<ShopApplicationDto>.Forbidden("You do not have permission to access this shop's data.");

        var shops = _uow.Repository<Shop>();
        var shop = await shops.FirstOrDefaultAsync(new ShopByIdSpecification(shopId), ct);
        if (shop is null)
            return Result<ShopApplicationDto>.NotFound("This shop could not be found.");

        shop.Name = request.Name.Trim();
        shop.Description = request.Description;
        shop.AddressLine = request.AddressLine;
        shop.District = request.District;
        shop.City = request.City;
        shop.PhoneNumber = request.PhoneNumber;
        shop.OpenTime = request.OpenTime;
        shop.CloseTime = request.CloseTime;
        shops.Update(shop);

        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<ShopApplicationDto>(shop));
    }
}
