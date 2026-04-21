using InstantProforms.Application.Features.Proforms.CreateProformShareLink;

namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Provides mappings for proform share link requests.
/// </summary>
public static class CreateProformShareLinkMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="proformId">The proform identifier.</param>
    /// <returns>The application command.</returns>
    public static CreateProformShareLinkCommand ToCommand(
        this CreateProformShareLinkRequest request,
        Guid proformId)
    {
        return new CreateProformShareLinkCommand(
            proformId,
            request.IsSingleUse,
            request.ExpirationMinutes);
    }
}