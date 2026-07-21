using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// A customer applies to open a shop. The application enters the onboarding state
/// machine at PendingReview and awaits administrator review (UC-25).
/// </summary>
public class ShopOnboardingService : IShopOnboardingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ShopOnboardingService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<ShopApplicationDto>> ApplyAsync(int userId, ShopApplicationRequest request, CancellationToken ct = default)
    {
        var shops = _uow.Repository<Shop>();

        // BR-66: at most one shop in PendingReview or Active per user.
        if (await shops.AnyAsync(new BlockingShopByOwnerSpecification(userId), ct))
            return Result<ShopApplicationDto>.Conflict("You already have a shop application under review or an active shop.");

        var shop = new Shop
        {
            OwnerId = userId,
            Name = request.Name.Trim(),
            Description = request.Description,
            AddressLine = request.AddressLine,
            District = request.District,
            City = request.City,
            PhoneNumber = request.PhoneNumber,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            Status = ShopStatus.PendingReview
        };

        await shops.AddAsync(shop, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(_mapper.Map<ShopApplicationDto>(shop));
    }

    public async Task<Result<IReadOnlyList<ShopApplicationDto>>> GetMyShopsAsync(int userId, CancellationToken ct = default)
    {
        var shops = await _uow.Repository<Shop>().ListAsync(new OwnedShopsSpecification(userId), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<ShopApplicationDto>>(shops));
    }
}
