using PrintHub.Domain.Enums;

namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Ambient information about the authenticated caller, resolved from the access
/// token. Implemented in the API layer over HttpContext. Services rely on this
/// rather than trusting client-supplied identifiers (BR-13).
/// </summary>
public interface ICurrentUser
{
    int? UserId { get; }
    string? Email { get; }
    UserRole? Role { get; }

    /// <summary>Shop ids the caller owns or is active staff of — the basis of scoped authorization.</summary>
    IReadOnlyCollection<int> ShopIds { get; }

    bool IsAuthenticated { get; }

    /// <summary>True if the caller owns or is staff of the given shop.</summary>
    bool BelongsToShop(int shopId);
}
