namespace InstantProforms.Api.Contracts.Auth;

public sealed record RegisterCompanyRequest(
    string CompanyName,
    string CompanySlug,
    string CompanyEmail,
    string? CompanyPhone,
    string? CompanyAddress,
    string OwnerFullName,
    string OwnerEmail,
    string? CompanyWebsite,
    string Password);