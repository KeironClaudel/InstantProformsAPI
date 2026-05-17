using InstantProforms.Application.Common.Models;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void GenerateRefreshToken_ReturnsUrlSafeToken()
    {
        var service = new JwtTokenService(Options.Create(new JwtSettings
        {
            SecretKey = "test-secret-key-should-be-long-enough",
            Issuer = "InstantProforms",
            Audience = "InstantProformsClient",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        }));

        var token = service.GenerateRefreshToken();

        Assert.DoesNotContain("+", token, StringComparison.Ordinal);
        Assert.DoesNotContain("/", token, StringComparison.Ordinal);
        Assert.DoesNotContain("=", token, StringComparison.Ordinal);
    }
}
