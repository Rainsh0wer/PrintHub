using AutoMapper;
using PrintHub.Application.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Shops.Dtos;
using PrintHub.Application.Specifications.Staff;
using PrintHub.Application.Specifications.Users;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Shops;

/// <summary>
/// Grants, revokes, and lists shop staff (UC-29). Owner-scoped (BR-74). Granting
/// elevates a plain customer to ShopStaff and creates the membership that
/// makes scoped authorization work; revoking is a soft deactivation so the actor
/// identity on past actions is preserved (BR-80).
/// </summary>
public class ShopStaffService : IShopStaffService
{
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogService _audit;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ShopStaffService(IUnitOfWork uow, IAuditLogService audit, ICurrentUser currentUser, IMapper mapper)
    {
        _uow = uow;
        _audit = audit;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<StaffDto>>> ListAsync(int shopId, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result<IReadOnlyList<StaffDto>>.Forbidden("You do not have permission to access this shop's data.");

        var staff = await _uow.Repository<ShopStaff>().ListAsync(new StaffByShopSpecification(shopId), ct);
        return Result.Success(_mapper.Map<IReadOnlyList<StaffDto>>(staff));
    }

    public async Task<Result> GrantAsync(int shopId, GrantStaffRequest request, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        // BR-79: the owner already has full access and cannot be their own staff.
        var target = await _uow.Repository<User>().FirstOrDefaultAsync(new UserByEmailSpecification(request.Email), ct);
        if (target is null)
            return Result.NotFound("No account was found with this email address.");
        if (target.Id == _currentUser.UserId)
            return Result.Conflict("You already have full access to your own shop.");
        if (target.Status != UserStatus.Active)
            return Result.Conflict("This account cannot be granted access.");

        var memberships = _uow.Repository<ShopStaff>();
        var existing = await memberships.FirstOrDefaultAsync(new StaffMembershipSpecification(shopId, target.Id), ct);

        if (existing is not null)
        {
            if (existing.IsActive)
                return Result.Conflict("This person is already a staff member of your shop.");

            // Reactivate a previously revoked membership (respects the unique (shop,user) index).
            existing.IsActive = true;
            existing.Position = request.Position;
            existing.JoinedAt = DateTime.UtcNow;
            memberships.Update(existing);
        }
        else
        {
            await memberships.AddAsync(new ShopStaff
            {
                ShopId = shopId,
                UserId = target.Id,
                Position = request.Position,
                JoinedAt = DateTime.UtcNow,
                IsActive = true
            }, ct);
        }

        // Elevate a plain customer to ShopStaff (owners/staff keep their role).
        if (target.Role == UserRole.Customer)
        {
            target.Role = UserRole.ShopStaff;
            _uow.Repository<User>().Update(target);
        }

        await _audit.AddAsync("GrantShopStaff", nameof(ShopStaff), $"{shopId}:{target.Id}", newValue: new { target.Email, request.Position }, ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> RevokeAsync(int shopId, int staffId, CancellationToken ct = default)
    {
        if (!_currentUser.CanManageShop(shopId))
            return Result.Forbidden("You do not have permission to access this shop's data.");

        var memberships = _uow.Repository<ShopStaff>();
        var membership = await memberships.FirstOrDefaultAsync(new StaffByIdSpecification(staffId), ct);
        if (membership is null || membership.ShopId != shopId)
            return Result.NotFound("This staff member could not be found.");

        // BR-80: soft revoke keeps historical records intact.
        membership.IsActive = false;
        memberships.Update(membership);

        await _audit.AddAsync("RevokeShopStaff", nameof(ShopStaff), membership.Id.ToString(), ct: ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
