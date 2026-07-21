namespace PrintHub.Infrastructure.Security;

/// <summary>Strongly-typed JWT settings bound from the "Jwt" configuration section (Options pattern).</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "PrintHub";
    public string Audience { get; set; } = "PrintHubClients";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
