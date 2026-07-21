namespace PrintHub.Application.Common.Interfaces;

/// <summary>
/// Hashes and verifies passwords. Implemented with BCrypt in Infrastructure so
/// the Application layer never depends on a specific hashing library.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
