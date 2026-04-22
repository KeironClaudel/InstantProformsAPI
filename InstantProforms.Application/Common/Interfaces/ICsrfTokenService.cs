namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines CSRF token generation operations.
/// </summary>
public interface ICsrfTokenService
{
    /// <summary>
    /// Generates a new CSRF token value.
    /// </summary>
    /// <returns>The generated token.</returns>
    string GenerateToken();
}