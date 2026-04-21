using InstantProforms.Application.Features.Auth.ResetPassword;

namespace InstantProforms.Api.Contracts.Auth;

/// <summary>
/// Provides mappings for reset password requests.
/// </summary>
public static class ResetPasswordMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static ResetPasswordCommand ToCommand(this ResetPasswordRequest request)
    {
        return new ResetPasswordCommand(request.Token, request.NewPassword);
    }
}