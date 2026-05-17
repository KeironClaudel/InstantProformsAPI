using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.CompanyConfig.GetCompanyLogo;
using InstantProforms.Domain.Entities;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.CompanyConfig;

public sealed class GetCompanyLogoQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsLogoBytesFromStorage()
    {
        var companyId = Guid.NewGuid();
        var storedFileId = Guid.NewGuid();
        const string relativeLogoPath = "company-logos/4cfd35ae-c59c-4b62-b51e-1911e15d4579/594d3686-d837-4def-a9b2-54f86a9f6059.png";
        var content = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        var companySettingsRepository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        companySettingsRepository
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanySettings
            {
                CompanyId = companyId,
                LogoFileName = relativeLogoPath,
                LogoStoredFileId = storedFileId
            });

        var storedFileRepository = new Mock<IStoredFileRepository>(MockBehavior.Strict);
        storedFileRepository
            .Setup(x => x.GetByIdAsync(storedFileId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StoredFile
            {
                Id = storedFileId,
                CompanyId = companyId,
                StoredFileName = "594d3686-d837-4def-a9b2-54f86a9f6059.png",
                ContentType = "text/html"
            });

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.SetupGet(x => x.StoredFiles).Returns(storedFileRepository.Object);

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);
        fileStorageService
            .Setup(x => x.GetBytesAsync(relativeLogoPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);

        var handler = new GetCompanyLogoQueryHandler(
            unitOfWork.Object,
            currentUserService.Object,
            fileStorageService.Object);

        var result = await handler.Handle(new GetCompanyLogoQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(content, result.Content);
        Assert.Equal("image/png", result.ContentType);
        Assert.Equal("594d3686-d837-4def-a9b2-54f86a9f6059.png", result.FileName);
    }

    [Fact]
    public async Task Handle_FallsBackToOctetStream_WhenStoredContentIsNotARecognizedImage()
    {
        var companyId = Guid.NewGuid();
        const string relativeLogoPath = "company-logos/demo/logo.png";
        var content = "<html>owned</html>"u8.ToArray();

        var companySettingsRepository = new Mock<ICompanySettingsRepository>(MockBehavior.Strict);
        companySettingsRepository
            .Setup(x => x.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanySettings
            {
                CompanyId = companyId,
                LogoFileName = relativeLogoPath
            });

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.CompanySettings).Returns(companySettingsRepository.Object);
        unitOfWork.SetupGet(x => x.StoredFiles).Returns(Mock.Of<IStoredFileRepository>());

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var fileStorageService = new Mock<IFileStorageService>(MockBehavior.Strict);
        fileStorageService
            .Setup(x => x.GetBytesAsync(relativeLogoPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(content);

        var handler = new GetCompanyLogoQueryHandler(
            unitOfWork.Object,
            currentUserService.Object,
            fileStorageService.Object);

        var result = await handler.Handle(new GetCompanyLogoQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("application/octet-stream", result.ContentType);
    }
}
