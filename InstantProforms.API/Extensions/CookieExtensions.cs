using Microsoft.AspNetCore.Http;

namespace InstantProforms.Api.Extensions;

public static class CookieExtensions
{
    public static void AppendAccessTokenCookie(this HttpResponse response, string token)
    {
        response.Cookies.Append("accessToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15),
            Path = "/"
        });
    }

    public static void AppendRefreshTokenCookie(this HttpResponse response, string token)
    {
        response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/"
        });
    }

    public static void ClearAuthCookies(this HttpResponse response)
    {
        response.Cookies.Delete("accessToken");
        response.Cookies.Delete("refreshToken");
    }
}