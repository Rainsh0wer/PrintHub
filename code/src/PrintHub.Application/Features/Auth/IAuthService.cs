using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Auth.Dtos;

namespace PrintHub.Application.Features.Auth;

/// <summary>
/// Authentication use cases (UC-01..UC-05). Every method returns a Result so the
/// controller stays thin and never decides HTTP status codes on its own.
/// </summary>
public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken ct = default);
    Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default);
    Task<Result> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default);
}
