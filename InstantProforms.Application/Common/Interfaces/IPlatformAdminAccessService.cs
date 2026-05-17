namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Evaluates whether a user is allowed to access platform-wide administration features.
/// </summary>
public interface IPlatformAdminAccessService
{
    /// <summary>
    /// Determines whether the specified email belongs to a platform administrator.
    /// </summary>
    /// <param name="email">The user email address.</param>
    /// <returns><c>true</c> when the email is allowed to use platform administration features; otherwise <c>false</c>.</returns>
    bool IsPlatformAdmin(string? email);
}
