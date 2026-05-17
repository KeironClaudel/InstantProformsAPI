using InstantProforms.Application.Common.Files;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Entities;
using MediatR;

namespace InstantProforms.Application.Features.CompanyConfig.ReplaceLogo;

/// <summary>
/// Handles replacing the company logo.
/// </summary>
public sealed class ReplaceCompanyLogoCommandHandler
    : IRequestHandler<ReplaceCompanyLogoCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public ReplaceCompanyLogoCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(ReplaceCompanyLogoCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.CompanyId is null)
        {
            throw new InvalidOperationException("Company context not found.");
        }

        var companyId = _currentUserService.CompanyId.Value;

        var settings = await _unitOfWork.CompanySettings
            .GetByCompanyIdAsync(companyId, cancellationToken);

        if (settings is null)
        {
            throw new InvalidOperationException("Company settings not found.");
        }

        if (!string.IsNullOrWhiteSpace(settings.LogoFileName))
        {
            await _fileStorageService.DeleteAsync(settings.LogoFileName, cancellationToken);
        }

        if (!ImageFileInspector.TryGetFormat(request.LogoFile, out var format) || format is null)
        {
            throw new InvalidOperationException("The uploaded logo is not a supported image.");
        }

        FileStorageSaveResult saveResult;

        await using (var stream = request.LogoFile.OpenReadStream())
        {
            saveResult = await _fileStorageService.SaveCompanyLogoAsync(
                companyId,
                $"logo{format.Extension}",
                stream,
                cancellationToken);
        }

        var utcNow = DateTime.UtcNow;

        var storedFile = new StoredFile
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            OriginalFileName = request.LogoFile.FileName,
            StoredFileName = saveResult.StoredFileName,
            RelativePath = saveResult.RelativePath,
            ContentType = format.ContentType,
            SizeBytes = request.LogoFile.Length,
            Purpose = "company-logo",
            CreatedAtUtc = utcNow
        };

        settings.LogoFileName = saveResult.RelativePath;
        settings.LogoStoredFileId = storedFile.Id;
        settings.UpdatedAtUtc = utcNow;

        await _unitOfWork.StoredFiles.AddAsync(storedFile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
