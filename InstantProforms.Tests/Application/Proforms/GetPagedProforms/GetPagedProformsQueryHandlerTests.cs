using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Features.Proforms.GetPagedProforms;
using InstantProforms.Domain.Entities;
using InstantProforms.Domain.Enums;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.GetPagedProforms;

public sealed class GetPagedProformsQueryHandlerTests
{
    [Fact]
    public async Task Handle_PassesFiltersAndPaginationToRepository()
    {
        var companyId = Guid.NewGuid();
        var expectedItems = Array.Empty<Proform>();

        var proformsRepository = new Mock<IProformRepository>(MockBehavior.Strict);
        proformsRepository
            .Setup(x => x.GetPagedAsync(
                companyId,
                2,
                25,
                "claudel",
                ProformStatus.Sent,
                new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2026, 5, 10, 23, 59, 59, 999, DateTimeKind.Utc).AddTicks(9999),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((expectedItems, 0));

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.Proforms).Returns(proformsRepository.Object);

        var currentUserService = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserService.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUserService.SetupGet(x => x.CompanyId).Returns(companyId);

        var handler = new GetPagedProformsQueryHandler(
            unitOfWork.Object,
            currentUserService.Object);

        var response = await handler.Handle(
            new GetPagedProformsQuery(
                Page: 2,
                PageSize: 25,
                ClientName: "claudel",
                Status: ProformStatus.Sent,
                FromDate: new DateOnly(2026, 5, 1),
                ToDate: new DateOnly(2026, 5, 10)),
            CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(2, response.Page);
        Assert.Equal(25, response.PageSize);
        Assert.Equal(0, response.TotalCount);
    }
}
