using InstantProforms.Application.Features.Clients.CreateClient;
using InstantProforms.Application.Features.Clients.UpdateClient;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Api.Contracts.Clients;

/// <summary>
/// Provides mappings for client HTTP contracts.
/// </summary>
public static class ClientMappings
{
    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <returns>The application command.</returns>
    public static CreateClientCommand ToCommand(this CreateClientRequest request)
    {
        return new CreateClientCommand(
            request.Name,
            request.Email,
            request.Phone,
            ParseIdentificationType(request.IdentificationType),
            request.IdentificationNumber);
    }

    /// <summary>
    /// Converts an HTTP request into an application command.
    /// </summary>
    /// <param name="request">The HTTP request.</param>
    /// <param name="clientId">The client identifier.</param>
    /// <returns>The application command.</returns>
    public static UpdateClientCommand ToCommand(this UpdateClientRequest request, Guid clientId)
    {
        return new UpdateClientCommand(
            clientId,
            request.Name,
            request.Email,
            request.Phone,
            ParseIdentificationType(request.IdentificationType),
            request.IdentificationNumber);
    }

    private static ClientIdentificationType? ParseIdentificationType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (Enum.TryParse<ClientIdentificationType>(value.Trim(), ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException("Unsupported client identification type.");
    }
}
