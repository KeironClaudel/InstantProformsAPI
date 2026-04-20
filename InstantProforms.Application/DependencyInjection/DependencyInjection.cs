using Microsoft.Extensions.DependencyInjection;

namespace InstantProforms.Application.DependencyInjection;

/// <summary>
/// Provides dependency injection registrations for the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}