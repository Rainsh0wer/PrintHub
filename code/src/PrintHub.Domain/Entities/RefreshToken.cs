using PrintHub.Domain.Common;

namespace PrintHub.Domain.Entities;

/// <summary>
/// Server-side refresh token record. Revoked on logout, password change, and
/// account lock — client-side removal alone is not sufficient.
/// </summary>
public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    /// <summary>The token that replaced this one on rotation (audit chain).</summary>
    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;

    // Navigation
    public User User { get; set; } = null!;
}
