using Microsoft.AspNetCore.Http;

namespace InstantProforms.Api.Services;

public interface IAuthCookieService
{
    void AppendSessionCookies(HttpResponse response, string accessToken, string refreshToken, string csrfToken, bool isPersistent);

    void AppendCsrfCookie(HttpResponse response, string csrfToken, bool isPersistent = false);

    void ClearSessionCookies(HttpResponse response);
}
