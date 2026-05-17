using System.Security.Cryptography;
using System.Text;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Protects secrets at rest using AES-GCM and an application master key.
/// </summary>
public sealed class AesGcmSecretProtector : ISecretProtector
{
    private const int NonceLength = 12;
    private const int TagLength = 16;
    private const byte PayloadVersion = 1;

    private readonly byte[] _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="AesGcmSecretProtector"/> class.
    /// </summary>
    /// <param name="settings">The protection settings.</param>
    public AesGcmSecretProtector(IOptions<SecretProtectionSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _key = ParseKey(settings.Value.MasterKey);
    }

    /// <inheritdoc />
    public string Protect(string plaintext)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plaintext);

        var nonce = RandomNumberGenerator.GetBytes(NonceLength);
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[TagLength];

        using var aesGcm = new AesGcm(_key, TagLength);
        aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var payload = new byte[1 + NonceLength + TagLength + ciphertext.Length];
        payload[0] = PayloadVersion;
        Buffer.BlockCopy(nonce, 0, payload, 1, NonceLength);
        Buffer.BlockCopy(tag, 0, payload, 1 + NonceLength, TagLength);
        Buffer.BlockCopy(ciphertext, 0, payload, 1 + NonceLength + TagLength, ciphertext.Length);

        return Convert.ToBase64String(payload);
    }

    /// <inheritdoc />
    public string Unprotect(string protectedValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(protectedValue);

        byte[] payload;

        try
        {
            payload = Convert.FromBase64String(protectedValue);
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException("The protected value is not a valid Base64 payload.", exception);
        }

        if (payload.Length < 1 + NonceLength + TagLength || payload[0] != PayloadVersion)
        {
            throw new InvalidOperationException("The protected value has an invalid format.");
        }

        var nonce = payload.AsSpan(1, NonceLength);
        var tag = payload.AsSpan(1 + NonceLength, TagLength);
        var ciphertext = payload.AsSpan(1 + NonceLength + TagLength);
        var plaintext = new byte[ciphertext.Length];

        try
        {
            using var aesGcm = new AesGcm(_key, TagLength);
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }
        catch (CryptographicException exception)
        {
            throw new InvalidOperationException("The protected value could not be decrypted.", exception);
        }

        return Encoding.UTF8.GetString(plaintext);
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
