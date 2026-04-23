using InstantProforms.Api.Common.Extensions;
using InstantProforms.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace InstantProforms.Api.Services;

public sealed class AuthCookieService : IAuthCookieService
{
    private const string LegacyCsrfCookiePath = "/api/auth";
    private readonly JwtSettings _jwtSettings;

    public AuthCookieService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public void AppendSessionCookies(HttpResponse response, string accessToken, string refreshToken, string csrfToken)
    {
        response.Cookies.Append("accessToken", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Path = "/"
        });

        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            Path = "/"
        });

        AppendCsrfCookie(response, csrfToken);
    }

    public void AppendCsrfCookie(HttpResponse response, string csrfToken)
    {
        DeleteLegacyCsrfCookie(response);

        response.Cookies.Append(CsrfCookieExtensions.CsrfCookieName, csrfToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Path = "/"
        });
    }

    public void ClearSessionCookies(HttpResponse response)
    {
        response.Cookies.Delete("accessToken", new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });

        response.Cookies.Delete(CsrfCookieExtensions.CsrfCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Path = "/"
        });

        DeleteLegacyCsrfCookie(response);
    }

    private static void DeleteLegacyCsrfCookie(HttpResponse response)
    {
        response.Cookies.Delete(CsrfCookieExtensions.CsrfCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true,
            Path = LegacyCsrfCookiePath
        });
    }
}
