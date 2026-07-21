using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Security;

/// <summary>
/// Issues signed JWT access tokens and opaque refresh tokens. The access token
/// carries identity, role, and one "shop" claim per shop the caller belongs to —
/// the raw material for scoped authorization.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    public const string ShopClaimType = "shop";

    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options) => _options = options.Value;

    public TokenResult CreateTokens(User user, IReadOnlyCollection<int> shopIds)
    {
        var now = DateTime.UtcNow;
        var accessExpires = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExpires = now.AddDays(_options.RefreshTokenDays);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(shopIds.Select(id => new Claim(ShopClaimType, id.ToString())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExpires,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new TokenResult(accessToken, accessExpires, refreshToken, refreshExpires);
    }
}
