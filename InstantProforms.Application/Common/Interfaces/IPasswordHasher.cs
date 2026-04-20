namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines password hashing operations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain text password against a hash.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <param name="passwordHash">The password hash.</param>
    /// <returns><c>true</c> when the password matches; otherwise <c>false</c>.</returns>
    bool VerifyPassword(string password, string passwordHash);
}