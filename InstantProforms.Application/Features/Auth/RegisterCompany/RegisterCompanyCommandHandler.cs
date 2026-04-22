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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCompanyCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    public RegisterCompanyCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
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

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.CompanyName,
            Slug = request.CompanySlug,
            Email = request.CompanyEmail,
            Phone = request.CompanyPhone,
            Address = request.CompanyAddress,
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        var companySettings = new CompanySettings
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            DisplayName = company.Name,
            Website = request.CompanyWebsite,
            Phone = company.Phone,
            Email = company.Email,
            Address = company.Address,
            TermsAndConditions =
                        "La garantía no cubre daños por manipulación indebida.\n" +
                        "Toda falla debe ser reportada directamente a la empresa antes de su reparación o intervención.\n" +
                        "Las reparaciones por terceros sin previa aceptación por la empresa anulan la garantía.\n" +
                        "La empresa no responde por uso indebido o conexiones no autorizadas.",
            LogoFileName = null,
            PrimaryColor = "#1B2D5A",
            SecondaryColor = "#e6c7f0",
            AccentColor = "#dbe2ff",
            ProformPrefix = "PRO",
            CurrencySymbol = "₡",
            TaxLabel = "Total",
            CreatedAtUtc = utcNow
        };

        await _unitOfWork.CompanySettings.AddAsync(companySettings, cancellationToken);

        var ownerUser = new User
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            RoleId = ownerRole.Id,
            FullName = request.OwnerFullName,
            Email = request.OwnerEmail,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = utcNow
        };

        await _unitOfWork.Companies.AddAsync(company, cancellationToken);
        await _unitOfWork.Users.AddAsync(ownerUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterCompanyResponse(
            company.Id,
            ownerUser.Id,
            "Company and owner user registered successfully.");
    }
}