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

    private int RememberMeRefreshTokenLifetimeDays =>
        _jwtSettings.RememberMeRefreshTokenExpirationDays is > 0
            ? _jwtSettings.RememberMeRefreshTokenExpirationDays.Value
            : _jwtSettings.RefreshTokenExpirationDays;

    public void AppendSessionCookies(HttpResponse response, string accessToken, string refreshToken, string csrfToken, bool isPersistent)
    {
        response.Cookies.Append("accessToken", accessToken, BuildAccessTokenCookieOptions(isPersistent));

        response.Cookies.Append("refreshToken", refreshToken, BuildRefreshTokenCookieOptions(isPersistent));

        AppendCsrfCookie(response, csrfToken, isPersistent);
    }

    public void AppendCsrfCookie(HttpResponse response, string csrfToken, bool isPersistent = false)
    {
        DeleteLegacyCsrfCookie(response);

        response.Cookies.Append(CsrfCookieExtensions.CsrfCookieName, csrfToken, BuildCsrfCookieOptions(isPersistent));
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

    private CookieOptions BuildAccessTokenCookieOptions(bool isPersistent)
    {
        var options = CreateSharedCookieOptions(httpOnly: true);

        if (isPersistent)
        {
            options.Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);
        }

        return options;
    }

    private CookieOptions BuildRefreshTokenCookieOptions(bool isPersistent)
    {
        var options = CreateSharedCookieOptions(httpOnly: true);

        if (isPersistent)
        {
            options.Expires = DateTimeOffset.UtcNow.AddDays(RememberMeRefreshTokenLifetimeDays);
        }

        return options;
    }

    private CookieOptions BuildCsrfCookieOptions(bool isPersistent)
    {
        var options = CreateSharedCookieOptions(httpOnly: false);
        options.IsEssential = true;

        if (isPersistent)
        {
            options.Expires = DateTimeOffset.UtcNow.AddDays(RememberMeRefreshTokenLifetimeDays);
        }

        return options;
    }

    private static CookieOptions CreateSharedCookieOptions(bool httpOnly)
    {
        return new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };
    }
}
