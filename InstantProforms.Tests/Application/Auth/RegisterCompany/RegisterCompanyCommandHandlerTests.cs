using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Auth.RegisterCompany;
using InstantProforms.Domain.Common;
using InstantProforms.Domain.Entities;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Auth.RegisterCompany;

public sealed class RegisterCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenLogoIsNotProvided_RegistersCompanyWithoutSavingStoredFile()
    {
        var companiesRepository = new Mock<ICompanyRepository>();
        companiesRepository
            .Setup(x => x.SlugExistsAsync("servicios-demo", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var usersRepository = new Mock<IUserRepository>();
        usersRepository
            .Setup(x => x.EmailExistsAsync("owner@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rolesRepository = new Mock<IRoleRepository>();
        rolesRepository
            .Setup(x => x.GetActiveByIdAsync(RoleIds.Owner, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role
            {
                Id = RoleIds.Owner,
                Name = "Owner",
                IsActive = true
            });

        var companySettingsRepository = new Mock<ICompanySettingsRepository>();
        var storedFilesRepository = new Mock<IStoredFileRepository>();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.SetupGet(x => x.Companies).Returns(companiesRepository.Object);
        unitOfWork.SetupGet(x => x.Users).Returns(usersRepository.Object);
        unitOfWork.SetupGet(x => x.Roles).Returns(rolesRepository.Object);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.SetupGet(x => x.StoredFiles).Returns(storedFilesRepository.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(x => x.HashPassword("Test1234")).Returns("hashed-password");

        var fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);

        var handler = new RegisterCompanyCommandHandler(
            unitOfWork.Object,
            passwordHasher.Object,
            fileStorageService.Object);

        var command = new RegisterCompanyCommand(
            CompanyName: "Servicios Demo",
            CompanySlug: "servicios-demo",
            CompanyEmail: "empresa@test.com",
            CompanyPhone: "2222-3333",
            CompanyAddress: "San José",
            CompanyWebsite: "https://servicios.test",
            DisplayName: "Servicios Demo",
            LegalName: null,
            TermsAndConditions: "Términos base.",
            PrimaryColor: "#123456",
            SecondaryColor: "#abcdef",
            AccentColor: "#fedcba",
            ProformPrefix: "C",
            CurrencySymbol: "₡",
            TaxLabel: "IVA",
            TaxPercentage: 13m,
            LogoFile: null,
            OwnerFullName: "Owner Demo",
            OwnerEmail: "owner@test.com",
            Password: "Test1234");

        var response = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, response.CompanyId);
        Assert.NotEqual(Guid.Empty, response.UserId);

        storedFilesRepository.Verify(
            x => x.AddAsync(It.IsAny<StoredFile>(), It.IsAny<CancellationToken>()),
            Times.Never);

        companySettingsRepository.Verify(
            x => x.AddAsync(
                It.Is<CompanySettings>(settings =>
                    settings.LogoFileName == null &&
                    settings.LogoStoredFileId == null &&
                    settings.ProformPrefix == "C"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
