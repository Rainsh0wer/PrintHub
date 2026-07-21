using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Shops;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Implements the administrator side of the shop onboarding state machine.
/// Each transition is validated against the current status, audited, and — on
/// approval — elevates the applicant to the ShopOwner role. All writes commit in
/// one unit of work so the shop change, role change, and audit row are atomic.
/// </summary>
public class ShopAdminService : IShopAdminService
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ShopAdminService(IUnitOfWork uow, IAuditLogService audit, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<ShopAdminListItemDto>>> ListPendingApplicationsAsync(CancellationToken ct = default)
    {
        var shops = await _uow.Repository<Shop>()
            .ListAsync(new ShopsByStatusSpecification(ShopStatus.PendingReview), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<ShopAdminListItemDto>>(shops));
    }

    public async Task<Result> ApproveAsync(int shopId, CancellationToken ct = default)
    {
        var shops = _uow.Repository<Shop>();
        var shop = await shops.FirstOrDefaultAsync(new ShopByIdSpecification(shopId), ct);
        if (shop is null) return Result.NotFound("This shop could not be found.");

        // BR-98: only a PendingReview application can be decided.
        if (shop.Status != ShopStatus.PendingReview)
            return Result.Conflict("This record's status has changed and no longer permits this action.");

        shop.Status = ShopStatus.Active;
        shop.ReviewNote = null;
        shop.ApprovedAt = DateTime.UtcNow;
        shop.ApprovedBy = _currentUser.UserId;
        shops.Update(shop);

        // BR-96: elevate the applicant to ShopOwner (keeps ordering ability as customer).
        var owner = await _uow.Repository<User>().GetByIdAsync(shop.OwnerId, ct);
        if (owner is not null && owner.Role == UserRole.Customer)
        {
            owner.Role = UserRole.ShopOwner;
            _uow.Repository<User>().Update(owner);
        }

        await _audit.AddAsync("ApproveShop", nameof(Shop), shop.Id.ToString(), newValue: new { shop.Status }, ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RejectAsync(int shopId, string reason, CancellationToken ct = default)
    {
        var shops = _uow.Repository<Shop>();
        var shop = await shops.FirstOrDefaultAsync(new ShopByIdSpecification(shopId), ct);
        if (shop is null) return Result.NotFound("This shop could not be found.");

        if (shop.Status != ShopStatus.PendingReview)
            return Result.Conflict("This record's status has changed and no longer permits this action.");

        shop.Status = ShopStatus.Rejected;
        shop.ReviewNote = reason;
        shops.Update(shop);

        await _audit.AddAsync("RejectShop", nameof(Shop), shop.Id.ToString(), newValue: new { reason }, ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> SuspendAsync(int shopId, string reason, CancellationToken ct = default)
    {
        var shops = _uow.Repository<Shop>();
        var shop = await shops.FirstOrDefaultAsync(new ShopByIdSpecification(shopId), ct);
        if (shop is null) return Result.NotFound("This shop could not be found.");

        // BR-99: only an Active shop can be suspended. In-progress orders keep running (not touched here).
        if (shop.Status != ShopStatus.Active)
            return Result.Conflict("This record's status has changed and no longer permits this action.");

        shop.Status = ShopStatus.Suspended;
        shop.ReviewNote = reason;
        shops.Update(shop);

        await _audit.AddAsync("SuspendShop", nameof(Shop), shop.Id.ToString(), newValue: new { reason }, ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ReinstateAsync(int shopId, CancellationToken ct = default)
    {
        var shops = _uow.Repository<Shop>();
        var shop = await shops.FirstOrDefaultAsync(new ShopByIdSpecification(shopId), ct);
        if (shop is null) return Result.NotFound("This shop could not be found.");

        if (shop.Status != ShopStatus.Suspended)
            return Result.Conflict("This record's status has changed and no longer permits this action.");

        shop.Status = ShopStatus.Active;
        shop.ReviewNote = null;
        shops.Update(shop);

        await _audit.AddAsync("ReinstateShop", nameof(Shop), shop.Id.ToString(), ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
