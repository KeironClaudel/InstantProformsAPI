namespace InstantProforms.Api.Common.Extensions;

/// <summary>
/// Provides helper methods for CSRF cookies.
/// </summary>
public static class CsrfCookieExtensions
{
    public const string CsrfCookieName = "XSRF-TOKEN";
    public const string CsrfHeaderName = "X-CSRF-TOKEN";

    /// <summary>
    /// Appends the CSRF cookie to the response.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="token">The token value.</param>
    public static void AppendCsrfCookie(this HttpResponse response, string token)
    {
        response.Cookies.Append(CsrfCookieName, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true
        });
    }

    /// <summary>
    /// Deletes the CSRF cookie from the response.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    public static void DeleteCsrfCookie(this HttpResponse response)
    {
        response.Cookies.Delete(CsrfCookieName, new CookieOptions
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            IsEssential = true
        });
    }
}