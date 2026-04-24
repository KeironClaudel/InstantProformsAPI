using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace InstantProforms.Infrastructure.Persistence;

/// <summary>
/// Creates <see cref="AppDbContext"/> instances at design time for EF Core tools.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string ApiProjectName = "InstantProforms.API";

    /// <summary>
    /// Creates a new <see cref="AppDbContext"/> instance.
    /// </summary>
    /// <param name="args">The design-time arguments.</param>
    /// <returns>A configured <see cref="AppDbContext"/> instance.</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = ResolveApiProjectPath();
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var userSecretsId = TryGetUserSecretsId(apiProjectPath);

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

        var userSecretsFilePath = TryGetUserSecretsFilePath(userSecretsId);

        if (!string.IsNullOrWhiteSpace(userSecretsFilePath))
        {
            configurationBuilder.AddJsonFile(userSecretsFilePath, optional: true, reloadOnChange: false);
        }

        IConfiguration configuration = configurationBuilder
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing or empty. Configure it in user-secrets, appsettings, or environment variables.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string ResolveApiProjectPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            currentDirectory,
            Path.Combine(currentDirectory, "..", ApiProjectName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ApiProjectName)
        };

        foreach (var candidate in candidates.Select(Path.GetFullPath).Distinct())
        {
            if (File.Exists(Path.Combine(candidate, $"{ApiProjectName}.csproj")))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException(
            $"Could not locate the {ApiProjectName} project directory from '{currentDirectory}'.");
    }

    private static string? TryGetUserSecretsId(string apiProjectPath)
    {
        var projectFilePath = Path.Combine(apiProjectPath, $"{ApiProjectName}.csproj");

        if (!File.Exists(projectFilePath))
        {
            return null;
        }

        var projectDocument = XDocument.Load(projectFilePath);

        return projectDocument
            .Descendants("UserSecretsId")
            .Select(element => element.Value.Trim())
            .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
    }

    private static string? TryGetUserSecretsFilePath(string? userSecretsId)
    {
        if (string.IsNullOrWhiteSpace(userSecretsId))
        {
            return null;
        }

        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        if (!string.IsNullOrWhiteSpace(appDataPath))
        {
            return Path.Combine(appDataPath, "Microsoft", "UserSecrets", userSecretsId, "secrets.json");
        }

        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        if (string.IsNullOrWhiteSpace(userProfilePath))
        {
            return null;
        }

        return Path.Combine(userProfilePath, ".microsoft", "usersecrets", userSecretsId, "secrets.json");
    }
}
