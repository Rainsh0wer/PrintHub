using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Domain.Enums;
using PrintHub.Infrastructure.Security;

namespace PrintHub.Api.Common;

/// <summary>
/// Resolves the authenticated caller from the JWT on the current request. This
/// is the API-layer implementation of <see cref="ICurrentUser"/> the services
/// depend on, so identity always comes from the token, never from the client body.
/// </summary>
public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId =>
        int.TryParse(Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;

    public string? Email => Principal?.FindFirstValue(JwtRegisteredClaimNames.Email);

    public UserRole? Role =>
        Enum.TryParse<UserRole>(Principal?.FindFirstValue(ClaimTypes.Role), out var role) ? role : null;

    public IReadOnlyCollection<int> ShopIds =>
        Principal?.FindAll(JwtTokenService.ShopClaimType)
            .Select(c => int.TryParse(c.Value, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToArray()
        ?? Array.Empty<int>();

    public bool BelongsToShop(int shopId) => ShopIds.Contains(shopId);
}
