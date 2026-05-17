using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Application.Features.Proforms.CreateProformShareLink;
using InstantProforms.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.CreateProformShareLink;

public sealed class CreateProformShareLinkCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsPathBasedShareLink_WithoutQueryToken()
    {
        var companyId = Guid.NewGuid();
        var proformId = Guid.NewGuid();

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var proforms = new Mock<IProformRepository>(MockBehavior.Strict);
        proforms.Setup(x => x.GetByIdAsync(proformId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Proform
            {
                Id = proformId,
                CompanyId = companyId,
                Number = "PRO-001"
            });

        var shareTokens = new Mock<IProformShareTokenRepository>(MockBehavior.Strict);
        shareTokens.Setup(x => x.AddAsync(It.IsAny<ProformShareToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.Proforms).Returns(proforms.Object);
        unitOfWork.SetupGet(x => x.ProformShareTokens).Returns(shareTokens.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var jwtTokenService = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwtTokenService.Setup(x => x.GenerateRefreshToken()).Returns("token-safe-for-path");

        var tokenHashService = new Mock<ITokenHashService>(MockBehavior.Strict);
        tokenHashService.Setup(x => x.ComputeHash("token-safe-for-path")).Returns("hashed-token");

        var handler = new CreateProformShareLinkCommandHandler(
            unitOfWork.Object,
            currentUserService.Object,
            jwtTokenService.Object,
            tokenHashService.Object,
            Options.Create(new ProformShareSettings
            {
                PublicDownloadUrl = "https://instantproforms.app/api/public/proforms/download",
                DefaultExpirationMinutes = 60
            }));

        var response = await handler.Handle(
            new CreateProformShareLinkCommand(proformId, false, null),
            CancellationToken.None);

        Assert.Equal("https://instantproforms.app/api/public/proforms/download/token-safe-for-path", response.Url);
        Assert.DoesNotContain("?token=", response.Url, StringComparison.OrdinalIgnoreCase);
    }
}
