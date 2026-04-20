using BCrypt.Net;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides password hashing using BCrypt.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}