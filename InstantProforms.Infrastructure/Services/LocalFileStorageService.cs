using InstantProforms.Application.Common.Interfaces;

namespace InstantProforms.Infrastructure.Services;

/// <summary>
/// Provides local disk file storage operations.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    /// <inheritdoc />
    public async Task<string> SaveCompanyLogoAsync(
        Guid companyId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";

        var rootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "uploads", "company-logos", companyId.ToString());
        Directory.CreateDirectory(rootPath);

        var fullPath = Path.Combine(rootPath, storedFileName);

        await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await content.CopyToAsync(fileStream, cancellationToken);

        return Path.Combine(companyId.ToString(), storedFileName).Replace("\\", "/");
    }
}