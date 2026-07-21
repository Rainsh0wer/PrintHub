using PrintHub.Application.Common.Specifications;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Specifications.RefreshTokens;

/// <summary>Finds a refresh token by its value, eager-loading the owning user.</summary>
public sealed class RefreshTokenByValueSpecification : BaseSpecification<RefreshToken>
{
    public RefreshTokenByValueSpecification(string token)
        : base(t => t.Token == token)
    {
        AddInclude(t => t.User);
    }
}

/// <summary>All active (non-revoked, unexpired) refresh tokens of a user.</summary>
public sealed class ActiveRefreshTokensByUserSpecification : BaseSpecification<RefreshToken>
{
    public ActiveRefreshTokensByUserSpecification(int userId)
        : base(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow)
    {
    }
}
