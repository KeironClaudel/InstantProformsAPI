using MediatR;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

public sealed record RegisterCompanyCommand(
    string CompanyName,
    string CompanySlug,
    string CompanyEmail,
    string? CompanyPhone,
    string? CompanyAddress,
    string OwnerFullName,
    string OwnerEmail,
    string? CompanyWebsite,
    string Password) : IRequest<RegisterCompanyResponse>;