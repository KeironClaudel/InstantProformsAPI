using InstantProforms.Application.Features.Auth.ForgotPassword;

namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Provides mappings for forgot password requests.
/// </summary>
public static class ForgotPasswordMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static ForgotPasswordCommand ToCommand(this ForgotPasswordRequest request)
    {
        return new ForgotPasswordCommand(request.Email);
    }
}