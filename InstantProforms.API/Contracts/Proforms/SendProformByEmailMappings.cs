using InstantProforms.Application.Features.Proforms.SendProformByEmail;

namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Provides mappings for proform email delivery requests.
/// </summary>
public static class SendProformByEmailMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="proformId">The proform identifier.</param>
    /// <returns>The application command.</returns>
    public static SendProformByEmailCommand ToCommand(
        this SendProformByEmailRequest request,
        Guid proformId)
    {
        return new SendProformByEmailCommand(
            proformId,
            request.ToEmail,
            request.Subject,
            request.Message);
    }
}