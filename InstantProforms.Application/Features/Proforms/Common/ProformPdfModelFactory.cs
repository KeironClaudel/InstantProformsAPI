using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;

namespace InstantProforms.Application.Features.Proforms.Common;

/// <summary>
/// Creates PDF models for proforms.
/// </summary>
public static class ProformPdfModelFactory
{
    /// <summary>
    /// Creates a <see cref="ProformPdfModel"/> from the provided domain entities.
    /// </summary>
    /// <param name="proform">The proform entity.</param>
    /// <param name="settings">The company settings.</param>
    /// <returns>The PDF model.</returns>
    public static ProformPdfModel Create(Proform proform, CompanySettings settings)
    {
        return new ProformPdfModel(
            DisplayName: settings.DisplayName,
            Website: settings.Website,
            Phone: settings.Phone,
            Email: settings.Email,
            Address: settings.Address,
            LogoFileName: settings.LogoFileName,
            PrimaryColor: settings.PrimaryColor,
            SecondaryColor: settings.SecondaryColor,
            AccentColor: settings.AccentColor,
            CurrencySymbol: proform.Currency.GetSymbol(),
            TaxLabel: settings.TaxLabel,
            TermsAndConditions: settings.TermsAndConditions,
            ProformId: proform.Id,
            Number: proform.Number,
            Status: proform.Status.ToString(),
            ClientName: proform.ClientName,
            ClientEmail: proform.ClientEmail,
            ClientPhone: proform.ClientPhone,
            ClientIdentificationType: proform.ClientIdentificationType?.ToString(),
            ClientIdentificationNumber: proform.ClientIdentificationNumber,
            TaxPercentage: proform.TaxPercentage,
            TaxAmount: proform.TaxAmount,
            IssuedAtUtc: proform.IssuedAtUtc,
            Location: proform.Location,
            InternalNotes: proform.InternalNotes,
            ServiceDescription: proform.ServiceDescription,
            ScopeOfWork: proform.ScopeOfWork,
            ServiceConditions: proform.ServiceConditions,
            PaymentConditions: proform.PaymentConditions,
            Subtotal: proform.Subtotal,
            Total: proform.Total,
            Items: proform.Items
                .OrderBy(x => x.SortOrder)
                .Select(x => new ProformPdfItemModel(
                    x.Id,
                    x.Description,
                    x.Quantity,
                    x.UnitPrice,
                    x.Total,
                    x.SortOrder))
                .ToList());
    }
}
