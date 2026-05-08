using InstantProforms.Application.Features.Proforms.CreateProform;
using InstantProforms.Domain.Enums;

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
            request.ClientId,
            request.ClientName,
            request.ClientEmail,
            request.ClientPhone,
            request.Notes,
            request.Location,
            request.InternalNotes,
            ParseIdentificationType(request.ClientIdentificationType),
            request.ClientIdentificationNumber,
            ParseCurrency(request.Currency),
            request.ServiceDescription,
            request.ScopeOfWork,
            request.ServiceConditions,
            request.PaymentConditions,
            request.Items
                .Select(x => new CreateProformItemModel(
                    x.Description,
                    x.Quantity,
                    x.UnitPrice))
                .ToList());
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

    private static ProformCurrency? ParseCurrency(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (Enum.TryParse<ProformCurrency>(value.Trim(), ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        throw new InvalidOperationException("Unsupported proform currency.");
    }
}
