using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Application.Specifications.RefreshTokens;
using PrintHub.Application.Specifications.Users;
using PrintHub.Domain.Entities;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Users;

public class AdminUserService : IAdminUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public AdminUserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<UserListItemDto>>> SearchAsync(
        string? q, UserRole? role, UserStatus? status, PageRequest page, CancellationToken ct = default)
    {
        var repo = _uow.Repository<User>();
        var total = await repo.CountAsync(new UserSearchCountSpecification(q, role, status), ct);
        var users = await repo.ListAsync(new UserSearchSpecification(q, role, status, page.Skip, page.Take), ct);

        var items = _mapper.Map<IReadOnlyList<UserListItemDto>>(users);
        return Result.Success(new PagedResult<UserListItemDto>(items, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<UserListItemDto>> LockAsync(int userId, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<UserListItemDto>.NotFound("Account not found.");
        if (user.Role == UserRole.Admin)
            return Result<UserListItemDto>.Forbidden("An administrator account cannot be locked.");

        user.Status = UserStatus.Locked;
        _uow.Repository<User>().Update(user);

        // Locking revokes the account's active refresh tokens.
        var active = await _uow.Repository<RefreshToken>().ListAsync(new ActiveRefreshTokensByUserSpecification(userId), ct);
        foreach (var token in active)
        {
            token.RevokedAt = DateTime.UtcNow;
            _uow.Repository<RefreshToken>().Update(token);
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<UserListItemDto>(user));
    }

    public async Task<Result<UserListItemDto>> UnlockAsync(int userId, CancellationToken ct = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(userId, ct);
        if (user is null)
            return Result<UserListItemDto>.NotFound("Account not found.");

        user.Status = UserStatus.Active;
        _uow.Repository<User>().Update(user);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<UserListItemDto>(user));
    }
}
