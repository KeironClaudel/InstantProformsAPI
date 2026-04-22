namespace InstantProforms.Application.Common.Interfaces;

/// <summary>
/// Defines file storage operations.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a company logo file and returns the stored file name.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="fileName">The original file name.</param>
    /// <param name="content">The file content stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored file name.</returns>
    Task<string> SaveCompanyLogoAsync(
        Guid companyId,
        string fileName,
        Stream content,
        CancellationToken cancellationToken);
}