using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Models;
using Microsoft.Extensions.Options;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Evaluates whether an email address is allowed to use platform-level administration features.
/// </summary>
public sealed class PlatformAdminAccessService : IPlatformAdminAccessService
{
    private readonly HashSet<string> _allowedEmails;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlatformAdminAccessService"/> class.
    /// </summary>
    /// <param name="options">The platform admin settings.</param>
    public PlatformAdminAccessService(IOptions<PlatformAdminSettings> options)
    {
        _allowedEmails = options.Value.AllowedEmails
            .Where(static email => !string.IsNullOrWhiteSpace(email))
            .Select(NormalizeEmail)
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <inheritdoc />
    public bool IsPlatformAdmin(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return _allowedEmails.Contains(NormalizeEmail(email));
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToUpperInvariant();
    }
}
