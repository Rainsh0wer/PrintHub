using PrintHub.Application.Common.Interfaces;

namespace PrintHub.Infrastructure.Security;

/// <summary>BCrypt implementation of <see cref="IPasswordHasher"/> (per-user salt built in).</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // A malformed stored hash should read as "does not match", not throw.
            return false;
        }
    }
}
