using MediatR;
using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Domain.Common;
using InstantProforms.Domain.Entities;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

/// <summary>
/// Handles company registration and owner user creation.
/// </summary>
public sealed class RegisterCompanyCommandHandler
    : IRequestHandler<RegisterCompanyCommand, RegisterCompanyResponse>
{
    private const string LegacyProformPrefix = "PRO";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IFileStorageService _fileStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCompanyCommandHandler"/> class.
    /// </summary>
    public RegisterCompanyCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _fileStorageService = fileStorageService;
    }

    /// <inheritdoc />
    public async Task<RegisterCompanyResponse> Handle(
        RegisterCompanyCommand request,
        CancellationToken cancellationToken)
    {
        var slugExists = await _unitOfWork.Companies
            .SlugExistsAsync(request.CompanySlug, cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException("The company slug is already in use.");
        }

        var ownerEmailExists = await _unitOfWork.Users
            .EmailExistsAsync(request.OwnerEmail, cancellationToken);

        if (ownerEmailExists)
        {
            throw new InvalidOperationException("The owner email is already in use.");
        }

        var ownerRole = await _unitOfWork.Roles
            .GetActiveByIdAsync(RoleIds.Owner, cancellationToken);

        if (ownerRole is null)
        {
            throw new InvalidOperationException("The Owner role was not found.");
        }

        var utcNow = DateTime.UtcNow;
        var companyId = Guid.NewGuid();

        var company = new Company
        {
            Id = companyId,
            Name = request.CompanyName,
            Slug = request.CompanySlug,
            Email = request.CompanyEmail,
            Phone = request.CompanyPhone,
            Address = request.CompanyAddress,
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        FileStorageSaveResult logoSaveResult;
        await using (var logoStream = request.LogoFile.OpenReadStream())
        {
            logoSaveResult = await _fileStorageService.SaveCompanyLogoAsync(
                companyId,
                request.LogoFile.FileName,
                logoStream,
                cancellationToken);
        }

        var storedLogo = new StoredFile
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            OriginalFileName = request.LogoFile.FileName,
            StoredFileName = logoSaveResult.StoredFileName,
            RelativePath = logoSaveResult.RelativePath,
            ContentType = request.LogoFile.ContentType,
            SizeBytes = request.LogoFile.Length,
            Purpose = "company-logo",
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        var companySettings = new CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            DisplayName = request.DisplayName,
            LegalName = request.LegalName,
            Website = request.CompanyWebsite,
            Phone = request.CompanyPhone,
            Email = request.CompanyEmail,
            Address = request.CompanyAddress,
            TermsAndConditions = request.TermsAndConditions,
            LogoFileName = logoSaveResult.RelativePath,
            LogoStoredFileId = storedLogo.Id,
            PrimaryColor = request.PrimaryColor,
            SecondaryColor = request.SecondaryColor,
            AccentColor = request.AccentColor,
            ProformPrefix = string.IsNullOrWhiteSpace(request.ProformPrefix)
                ? LegacyProformPrefix
                : request.ProformPrefix.Trim(),
            CurrencySymbol = request.CurrencySymbol,
            TaxPercentage = request.TaxPercentage,
            TaxLabel = request.TaxLabel,
            CreatedAtUtc = utcNow
        };

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            RoleId = ownerRole.Id,
            FullName = request.OwnerFullName,
            Email = request.OwnerEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        await _unitOfWork.StoredFiles.AddAsync(storedLogo, cancellationToken);
        await _unitOfWork.Companies.AddAsync(company, cancellationToken);
        await _unitOfWork.CompanySettings.AddAsync(companySettings, cancellationToken);
        await _unitOfWork.Users.AddAsync(ownerUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCompanyResponse(
            company.Id,
            ownerUser.Id,
            "Company and owner user registered successfully.");
    }
}
