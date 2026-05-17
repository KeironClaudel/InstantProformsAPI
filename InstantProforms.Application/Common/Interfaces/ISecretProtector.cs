namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Protects and restores sensitive values using reversible encryption.
/// </summary>
public interface ISecretProtector
{
    /// <summary>
    /// Encrypts a plaintext value for safe storage.
    /// </summary>
    /// <param name="plaintext">The plaintext value.</param>
    /// <returns>The encrypted payload.</returns>
    string Protect(string plaintext);

    /// <summary>
    /// Decrypts a protected value.
    /// </summary>
    /// <param name="protectedValue">The encrypted payload.</param>
    /// <returns>The original plaintext value.</returns>
    string Unprotect(string protectedValue);
}
