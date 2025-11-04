using Application.Abstraction.Authentication;

namespace Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    // No hashing - store password as plain text for easier testing
    public string Hash(string password)
    {
        return password;
    }

    public bool Verify(string password, string passwordHash)
    {
        return password == passwordHash;
    }
}