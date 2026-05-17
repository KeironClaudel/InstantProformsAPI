using InstantProforms.Application.Common.Interfaces;
using InstantProforms.Application.Common.Interfaces.Persistence;
using InstantProforms.Application.Common.Models;
using InstantProforms.Application.Features.Auth.Login;
using InstantProforms.Application.Features.Auth.Logout;
using InstantProforms.Application.Features.Auth.RefToken;
using InstantProforms.Domain.Entities;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace InstantProforms.Tests.Application.Auth;

public sealed class RefreshTokenHandlersTests
{
    [Fact]
    public async Task Login_StoresOnlyHashedRefreshToken()
    {
        var user = CreateUser();
        RefreshToken? persistedToken = null;

        var users = new Mock<IUserRepository>(MockBehavior.Strict);
        users.Setup(x => x.GetActiveByEmailWithRoleAsync("owner@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var refreshTokens = new Mock<IRefreshTokenRepository>(MockBehavior.Strict);
        refreshTokens.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => persistedToken = token)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.Users).Returns(users.Object);
        unitOfWork.SetupGet(x => x.RefreshTokens).Returns(refreshTokens.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var passwordHasher = new Mock<IPasswordHasher>(MockBehavior.Strict);
        passwordHasher.Setup(x => x.VerifyPassword("ValidPass1", user.PasswordHash)).Returns(true);

        var jwtTokenService = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwtTokenService.Setup(x => x.GenerateAccessToken(user)).Returns("access-token");
        jwtTokenService.Setup(x => x.GenerateRefreshToken()).Returns("raw-refresh-token");

        var tokenHashService = new Mock<ITokenHashService>(MockBehavior.Strict);
        tokenHashService.Setup(x => x.ComputeHash("raw-refresh-token")).Returns("hashed-refresh-token");

        var handler = new LoginCommandHandler(
            unitOfWork.Object,
            passwordHasher.Object,
            jwtTokenService.Object,
            tokenHashService.Object,
            Options.Create(new JwtSettings
            {
                RefreshTokenExpirationDays = 7
            }));

        var response = await handler.Handle(new LoginCommand("owner@example.com", "ValidPass1"), CancellationToken.None);

        Assert.NotNull(persistedToken);
        Assert.Equal("hashed-refresh-token", persistedToken!.Token);
        Assert.Equal("raw-refresh-token", response.RefreshToken);
    }

    [Fact]
    public async Task Refresh_UsesHashedLookup_AndPersistsHashedReplacement()
    {
        var user = CreateUser();
        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Token = "existing-hash",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };

        RefreshToken? persistedToken = null;

        var refreshTokens = new Mock<IRefreshTokenRepository>(MockBehavior.Strict);
        refreshTokens.Setup(x => x.GetByTokenHashWithUserAsync("provided-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);
        refreshTokens.Setup(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Callback<RefreshToken, CancellationToken>((token, _) => persistedToken = token)
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.RefreshTokens).Returns(refreshTokens.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var jwtTokenService = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwtTokenService.Setup(x => x.GenerateAccessToken(user)).Returns("new-access-token");
        jwtTokenService.Setup(x => x.GenerateRefreshToken()).Returns("new-raw-refresh-token");

        var tokenHashService = new Mock<ITokenHashService>(MockBehavior.Strict);
        tokenHashService.Setup(x => x.ComputeHash("provided-raw-token")).Returns("provided-hash");
        tokenHashService.Setup(x => x.ComputeHash("new-raw-refresh-token")).Returns("new-refresh-hash");

        var handler = new RefreshTokenCommandHandler(
            unitOfWork.Object,
            jwtTokenService.Object,
            tokenHashService.Object,
            Options.Create(new JwtSettings
            {
                RefreshTokenExpirationDays = 7
            }));

        var response = await handler.Handle(new RefreshTokenCommand("provided-raw-token"), CancellationToken.None);

        Assert.NotNull(persistedToken);
        Assert.Equal("new-refresh-hash", persistedToken!.Token);
        Assert.NotNull(storedToken.RevokedAtUtc);
        Assert.Equal("new-raw-refresh-token", response.RefreshToken);
    }

    [Fact]
    public async Task Logout_UsesHashedLookup()
    {
        var storedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "logout-hash",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };

        var refreshTokens = new Mock<IRefreshTokenRepository>(MockBehavior.Strict);
        refreshTokens.Setup(x => x.GetByTokenHashAsync("logout-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        unitOfWork.SetupGet(x => x.RefreshTokens).Returns(refreshTokens.Object);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var tokenHashService = new Mock<ITokenHashService>(MockBehavior.Strict);
        tokenHashService.Setup(x => x.ComputeHash("logout-raw-token")).Returns("logout-hash");

        var handler = new LogoutCommandHandler(unitOfWork.Object, tokenHashService.Object);

        await handler.Handle(new LogoutCommand("logout-raw-token"), CancellationToken.None);

        Assert.NotNull(storedToken.RevokedAtUtc);
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Email = "owner@example.com",
            FullName = "Owner User",
            PasswordHash = "stored-hash",
            IsActive = true,
            Role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Owner"
            }
        };
    }
}
