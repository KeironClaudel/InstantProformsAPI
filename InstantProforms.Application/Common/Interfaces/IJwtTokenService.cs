using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines JWT token generation operations.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates an access token for the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>The JWT access token.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a secure refresh token value.
    /// </summary>
    /// <returns>A refresh token string.</returns>
    string GenerateRefreshToken();
}