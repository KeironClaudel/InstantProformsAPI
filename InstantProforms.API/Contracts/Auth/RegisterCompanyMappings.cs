using InstantProforms.Application.Features.Auth.RegisterCompany;

namespace InstantProforms.Api.Contracts.Auth;

public static class RegisterCompanyMappings
{
    public static RegisterCompanyCommand ToCommand(this RegisterCompanyRequest request)
    {
        return new RegisterCompanyCommand(
            request.CompanyName,
            request.CompanySlug,
            request.CompanyEmail,
            request.CompanyPhone,
            request.CompanyAddress,
            request.OwnerFullName,
            request.OwnerEmail,
            request.CompanyWebsite,
            request.Password);
    }
}