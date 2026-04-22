using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides local disk file storage operations.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
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
            AppContext.BaseDirectory,
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
        var fullPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}