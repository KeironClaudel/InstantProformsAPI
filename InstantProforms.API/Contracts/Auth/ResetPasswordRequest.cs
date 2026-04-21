namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Represents the HTTP request to reset a password.
/// </summary>
public sealed record ResetPasswordRequest(
    string Token,
    string NewPassword);