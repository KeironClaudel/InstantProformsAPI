using InstantProforms.Application.Features.Proforms.UpdateProformStatus;

namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Provides mappings for proform status update requests.
/// </summary>
public static class UpdateProformStatusMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="proformId">The proform identifier.</param>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static UpdateProformStatusCommand ToCommand(
        this UpdateProformStatusRequest request,
        Guid proformId)
    {
        return new UpdateProformStatusCommand(proformId, request.Status);
    }
}