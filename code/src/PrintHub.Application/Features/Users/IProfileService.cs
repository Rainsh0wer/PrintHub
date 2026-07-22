using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Users.Dtos;

namespace PrintHub.Application.Features.Users;

/// <summary>The caller's own account profile (UC-06 view, UC-07 update).</summary>
public interface IProfileService
{
    Task<Result<ProfileDto>> GetMeAsync(int userId, CancellationToken ct = default);
    Task<Result<ProfileDto>> UpdateMeAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default);
}
