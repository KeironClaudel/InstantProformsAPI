using System.Security.Cryptography;
using System.Text;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Computes deterministic keyed fingerprints for sensitive values.
/// </summary>
public sealed class HmacSecretFingerprintService : ISecretFingerprintService
{
    private readonly byte[] _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacSecretFingerprintService"/> class.
    /// </summary>
    /// <param name="settings">The protection settings.</param>
    public HmacSecretFingerprintService(IOptions<SecretProtectionSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _key = ParseKey(settings.Value.MasterKey);
    }

    /// <inheritdoc />
    public string ComputeFingerprint(string plaintext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintext);

        using var hmac = new HMACSHA256(_key);
        var fingerprint = hmac.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
        return Convert.ToHexString(fingerprint);
    }

    private static byte[] ParseKey(string masterKey)
    {
        if (string.IsNullOrWhiteSpace(masterKey))
        {
            throw new InvalidOperationException("SecretProtectionSettings:MasterKey is required.");
        }

        byte[] keyBytes;

        try
        {
            keyBytes = Convert.FromBase64String(masterKey);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "SecretProtectionSettings:MasterKey must be a Base64-encoded 32-byte key.",
                exception);
        }

        if (keyBytes.Length != 32)
        {
            throw new InvalidOperationException(
                "SecretProtectionSettings:MasterKey must decode to exactly 32 bytes.");
        }

        return keyBytes;
    }
}
