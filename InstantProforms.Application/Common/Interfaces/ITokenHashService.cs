namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines hashing operations for one-time tokens.
/// </summary>
public interface ITokenHashService
{
    /// <summary>
    /// Computes a deterministic hash for a token value.
    /// </summary>
    /// <param name="token">The raw token value.</param>
    /// <returns>The hashed token value.</returns>
    string ComputeHash(string token);
}