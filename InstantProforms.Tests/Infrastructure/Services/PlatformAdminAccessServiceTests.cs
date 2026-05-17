using InstantProforms.Application.Common.Models;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class PlatformAdminAccessServiceTests
{
    [Fact]
    public void IsPlatformAdmin_WhenEmailIsConfigured_ReturnsTrue()
    {
        var service = new PlatformAdminAccessService(
            Options.Create(new PlatformAdminSettings
            {
                AllowedEmails = new List<string> { "owner@example.com" }
            }));

        var result = service.IsPlatformAdmin(" Owner@Example.com ");

        Assert.True(result);
    }

    [Fact]
    public void IsPlatformAdmin_WhenEmailIsNotConfigured_ReturnsFalse()
    {
        var service = new PlatformAdminAccessService(
            Options.Create(new PlatformAdminSettings
            {
                AllowedEmails = new List<string> { "owner@example.com" }
            }));

        var result = service.IsPlatformAdmin("other@example.com");

        Assert.False(result);
    }
}
