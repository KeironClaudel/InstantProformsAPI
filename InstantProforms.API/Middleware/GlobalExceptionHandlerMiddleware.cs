using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace InstantProforms.Api.Middleware;

/// <summary>
/// Handles unhandled exceptions globally and returns standardized responses.
/// </summary>
public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandlerMiddleware"/> class.
    /// </summary>
    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Processes the HTTP request.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);

        var problem = CreateProblemDetails(context, exception, traceId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, string traceId)
    {
        var problem = new ProblemDetails
        {
            Instance = context.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        switch (exception)
        {
            case ValidationException validationException:
                problem.Title = "Validation error";
                problem.Status = StatusCodes.Status400BadRequest;
                problem.Detail = "One or more validation errors occurred.";

                problem.Extensions["errors"] = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                break;

            case UnauthorizedAccessException:
                problem.Title = "Unauthorized";
                problem.Status = StatusCodes.Status401Unauthorized;
                problem.Detail = "You are not authorized to access this resource.";
                break;

            case InvalidOperationException:
                problem.Title = "Bad request";
                problem.Status = StatusCodes.Status400BadRequest;
                problem.Detail = exception.Message;
                break;

            default:
                problem.Title = "Internal server error";
                problem.Status = StatusCodes.Status500InternalServerError;

                problem.Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred.";

                if (_environment.IsDevelopment())
                {
                    problem.Extensions["exceptionType"] = exception.GetType().Name;
                    problem.Extensions["stackTrace"] = exception.StackTrace;
                }
                break;
        }

        return problem;
    }
}