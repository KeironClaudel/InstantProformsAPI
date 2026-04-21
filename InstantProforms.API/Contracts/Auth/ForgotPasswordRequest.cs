namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Represents the HTTP request to start a forgot password flow.
/// </summary>
public sealed record ForgotPasswordRequest(string Email);