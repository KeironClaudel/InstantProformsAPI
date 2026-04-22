using System.Security.Cryptography;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides CSRF token generation using cryptographically secure random bytes.
/// </summary>
public sealed class CsrfTokenService : ICsrfTokenService
{
    /// <inheritdoc />
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}