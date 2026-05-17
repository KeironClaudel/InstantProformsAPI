using System.Text.Json;
using FluentValidation;
using InstantProforms.Api.Middleware;
using InstantProforms.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace InstantProforms.Tests.Api.Middleware;

public sealed class GlobalExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithExternalServiceException_ReturnsSafeBadGatewayPayload()
    {
        var middleware = new GlobalExceptionHandlerMiddleware(
            _ => throw new ExternalServiceException("Email delivery failed. Please try again later."),
            NullLogger<GlobalExceptionHandlerMiddleware>.Instance,
            new FakeHostEnvironment());

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status502BadGateway, context.Response.StatusCode);
        context.Response.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("External service error", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("Email delivery failed. Please try again later.", document.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WithConfigurationException_ReturnsSafeBadRequestPayload()
    {
        var middleware = new GlobalExceptionHandlerMiddleware(
            _ => throw new ConfigurationException("Email delivery is not configured for this company."),
            NullLogger<GlobalExceptionHandlerMiddleware>.Instance,
            new FakeHostEnvironment());

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        context.Response.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("Configuration error", document.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WithSafeInvalidOperationException_ReturnsOriginalMessage()
    {
        var middleware = new GlobalExceptionHandlerMiddleware(
            _ => throw new InvalidOperationException("Proform was not found."),
            NullLogger<GlobalExceptionHandlerMiddleware>.Instance,
            new FakeHostEnvironment());

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        context.Response.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("Bad request", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("Proform was not found.", document.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WithUnsafeInvalidOperationException_HidesSensitiveMessage()
    {
        var middleware = new GlobalExceptionHandlerMiddleware(
            _ => throw new InvalidOperationException("Connection string 'DefaultConnection' was not found."),
            NullLogger<GlobalExceptionHandlerMiddleware>.Instance,
            new FakeHostEnvironment());

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        context.Response.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal("Bad request", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("The requested operation could not be completed.", document.RootElement.GetProperty("detail").GetString());
    }

    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Production";
        public string ApplicationName { get; set; } = "InstantProforms.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
