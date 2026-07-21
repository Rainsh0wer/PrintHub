using PrintHub.Domain.Entities;

namespace PrintHub.Application.Common.Interfaces;

/// <summary>A freshly issued access/refresh token pair with their expiry times.</summary>
public record TokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

/// <summary>
/// Issues JWT access tokens and opaque refresh tokens. The access token carries
/// the user's identity, role, and shop-membership claims used for scoped
/// authorization. Implemented in Infrastructure.
/// </summary>
public interface IJwtTokenService
{
    TokenResult CreateTokens(User user, IReadOnlyCollection<int> shopIds);
}
