using MediatR;
using Microsoft.AspNetCore.Http;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

/// <summary>
/// Represents a request to register a company, its owner user, and branding settings.
/// </summary>
public sealed record RegisterCompanyCommand(
    string CompanyName,
    string CompanySlug,
    string? CompanyEmail,
    string? CompanyPhone,
    string? CompanyAddress,
    string? CompanyWebsite,
    string DisplayName,
    string? LegalName,
    string TermsAndConditions,
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string ProformPrefix,
    string CurrencySymbol,
    string TaxLabel,
    IFormFile LogoFile,
    string OwnerFullName,
    string OwnerEmail,
    string Password) : IRequest<RegisterCompanyResponse>;