using InstantProforms.Api.Common.Extensions;
using InstantProforms.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace InstantProforms.Tests.Api.Middleware;

public sealed class CsrfValidationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_BlocksRefresh_WhenSessionCookieExistsButCsrfHeaderIsMissing()
    {
        var nextCalled = false;
        var middleware = new CsrfValidationMiddleware(context =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/auth/refresh";
        context.Request.Headers.Cookie = "refreshToken=demo-refresh-token; XSRF-TOKEN=csrf-cookie-token";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AllowsRefresh_WhenCookieAndHeaderTokensMatch()
    {
        var nextCalled = false;
        var middleware = new CsrfValidationMiddleware(context =>
        {
            nextCalled = true;
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        });

        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/auth/refresh";
        context.Request.Headers.Cookie = "refreshToken=demo-refresh-token; XSRF-TOKEN=csrf-cookie-token";
        context.Request.Headers[CsrfCookieExtensions.CsrfHeaderName] = "csrf-cookie-token";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }
}
