using Microsoft.AspNetCore.Http;

namespace InstantProforms.Api.Services;

public interface IAuthCookieService
{
    void AppendSessionCookies(HttpResponse response, string accessToken, string refreshToken, string csrfToken);

    void AppendCsrfCookie(HttpResponse response, string csrfToken);

    void ClearSessionCookies(HttpResponse response);
}
