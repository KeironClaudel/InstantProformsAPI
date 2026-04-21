using System.Security.Cryptography;
using System.Text;
using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides SHA256 hashing for one-time tokens.
/// </summary>
public sealed class Sha256TokenHashService : ITokenHashService
{
    /// <inheritdoc />
    public string ComputeHash(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}