using InstantProforms.Application.Features.Auth.Login;

namespace InstantProforms.Api.Contracts.Auth;

public static class LoginMappings
{
    public static LoginCommand ToCommand(this LoginRequest request)
    {
        return new LoginCommand(request.Email, request.Password);
    }
}