namespace InstantProforms.Application.Common.Models;

/// <summary>
/// Provides configuration for reversible secret protection.
/// </summary>
public sealed class SecretProtectionSettings
{
    /// <summary>
    /// Gets or sets the Base64-encoded 32-byte master key used to protect secrets at rest.
    /// </summary>
    public string MasterKey { get; set; } = string.Empty;
}
