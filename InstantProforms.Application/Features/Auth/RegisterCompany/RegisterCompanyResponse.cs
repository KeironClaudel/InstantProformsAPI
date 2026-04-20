namespace InstantProforms.Application.Features.Auth.RegisterCompany;

public sealed record RegisterCompanyResponse(
    Guid CompanyId,
    Guid UserId,
    string Message);