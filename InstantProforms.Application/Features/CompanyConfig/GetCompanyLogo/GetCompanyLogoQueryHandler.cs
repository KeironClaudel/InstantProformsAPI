using InstantProforms.Application.Common.Files;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using MediatR;

namespace InstantProforms.Application.Features.CompanyConfig.GetCompanyLogo;

/// <summary>
/// Handles retrieval of the current company logo content.
/// </summary>
public sealed class GetCompanyLogoQueryHandler : IRequestHandler<GetCompanyLogoQuery, GetCompanyLogoResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCompanyLogoQueryHandler"/> class.
    /// </summary>
    public GetCompanyLogoQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<GetCompanyLogoResponse?> Handle(GetCompanyLogoQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Authenticated company context was not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;
        var settings = await _unitOfWork.CompanySettings.GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null || string.IsNullOrWhiteSpace(settings.LogoFileName))
        {
            return null;
        }

        var content = await _fileStorageService.GetBytesAsync(settings.LogoFileName, cancellationToken);

        if (content is null || content.Length == 0)
        {
            return null;
        }

        var storedFile = settings.LogoStoredFileId is null
            ? null
            : await _unitOfWork.StoredFiles.GetByIdAsync(settings.LogoStoredFileId.Value, cancellationToken);

        var contentType = ImageFileInspector.TryGetFormat(content, out var format) && format is not null
            ? format.ContentType
            : "application/octet-stream";

        var fileName = storedFile?.CompanyId == companyId && !string.IsNullOrWhiteSpace(storedFile.StoredFileName)
            ? storedFile.StoredFileName
            : Path.GetFileName(settings.LogoFileName);

        return new GetCompanyLogoResponse(content, contentType, fileName);
    }
}
