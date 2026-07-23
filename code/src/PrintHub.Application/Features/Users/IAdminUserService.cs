using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;
using PrintHub.Domain.Enums;

namespace PrintHub.Application.Features.Users;

public interface IAdminUserService
{
    Task<Result<PagedResult<UserListItemDto>>> SearchAsync(string? q, UserRole? role, UserStatus? status, PageRequest page, CancellationToken ct = default);
    Task<Result<UserListItemDto>> LockAsync(int userId, CancellationToken ct = default);
    Task<Result<UserListItemDto>> UnlockAsync(int userId, CancellationToken ct = default);
}
