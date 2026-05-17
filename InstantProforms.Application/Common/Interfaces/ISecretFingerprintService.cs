namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Computes deterministic keyed fingerprints for sensitive values.
/// </summary>
public interface ISecretFingerprintService
{
    /// <summary>
    /// Computes a keyed fingerprint for lookup and uniqueness checks.
    /// </summary>
    /// <param name="plaintext">The plaintext value.</param>
    /// <returns>The deterministic fingerprint.</returns>
    string ComputeFingerprint(string plaintext);
}
