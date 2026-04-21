using InstantProforms.Api.Contracts.Proforms;
using InstantProforms.Application.Features.Proforms.CreateProform;

namespace InstantProforms.Api.Contracts.Proforms;

/// <summary>
/// Provides mappings for proform HTTP contracts.
/// </summary>
public static class CreateProformMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static CreateProformCommand ToCommand(this CreateProformRequest request)
    {
        return new CreateProformCommand(
            request.ClientName,
            request.ClientEmail,
            request.ClientPhone,
            request.Notes,
            request.Items
                .Select(x => new CreateProformItemModel(
                    x.Description,
                    x.Quantity,
                    x.UnitPrice))
                .ToList());
    }
}