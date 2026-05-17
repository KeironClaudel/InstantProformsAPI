using InstantProforms.Api.Services;
using InstantProforms.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Xunit;

namespace InstantProforms.Tests.Api.Services;

public sealed class AuthCookieServiceTests
{
    [Fact]
    public void AppendSessionCookies_WhenRememberMeIsDisabled_UsesSessionCookies()
    {
        var service = CreateService();
        var context = new DefaultHttpContext();

        service.AppendSessionCookies(context.Response, "access-token", "refresh-token", "csrf-token", false);

        var setCookieHeader = context.Response.Headers.SetCookie.ToString();

        Assert.Contains("accessToken=access-token; path=/; secure; samesite=none; httponly", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("refreshToken=refresh-token; path=/; secure; samesite=none; httponly", setCookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AppendSessionCookies_WhenRememberMeIsEnabled_UsesPersistentCookies()
    {
        var service = CreateService();
        var context = new DefaultHttpContext();

        service.AppendSessionCookies(context.Response, "access-token", "refresh-token", "csrf-token", true);

        var setCookieHeader = context.Response.Headers.SetCookie.ToString();

        Assert.Contains("accessToken=access-token", setCookieHeader);
        Assert.Contains("refreshToken=refresh-token", setCookieHeader);
        Assert.Contains("accessToken=access-token; expires=", setCookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("refreshToken=refresh-token; expires=", setCookieHeader, StringComparison.OrdinalIgnoreCase);
    }

    private static AuthCookieService CreateService()
    {
        return new AuthCookieService(Options.Create(new JwtSettings
        {
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7,
            RememberMeRefreshTokenExpirationDays = 30
        }));
    }
}
