using InstantProforms.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides local disk file storage operations.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileStorageService"/> class.
    /// </summary>
    /// <param name="environment">The web host environment.</param>
    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    /// <inheritdoc />
    public async Task<FileStorageSaveResult> SaveCompanyLogoAsync(
        Guid companyId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var rootPath = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            "uploads",
            "company-logos",
            companyId.ToString());

        Directory.CreateDirectory(rootPath);

        var fullPath = Path.Combine(rootPath, storedFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);

        var relativePath = Path.Combine("uploads", "company-logos", companyId.ToString(), storedFileName)
            .Replace("\\", "/");

        return new FileStorageSaveResult(storedFileName, relativePath);
    }

    /// <inheritdoc />
    public Task DeleteAsync(string relativePath)
    {
        var fullPath = Path.Combine(
            _environment.ContentRootPath,
            "wwwroot",
            relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}