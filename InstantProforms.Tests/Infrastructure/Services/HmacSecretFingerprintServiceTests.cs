using InstantProforms.Application.Common.Models;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class HmacSecretFingerprintServiceTests
{
    [Fact]
    public void ComputeFingerprint_IsDeterministicForSameInput()
    {
        var service = new HmacSecretFingerprintService(CreateOptions());

        var first = service.ComputeFingerprint("3-101-999999");
        var second = service.ComputeFingerprint("3-101-999999");

        Assert.Equal(first, second);
    }

    [Fact]
    public void ComputeFingerprint_ChangesForDifferentInput()
    {
        var service = new HmacSecretFingerprintService(CreateOptions());

        var first = service.ComputeFingerprint("3-101-999999");
        var second = service.ComputeFingerprint("1-2345-6789");

        Assert.NotEqual(first, second);
    }

    private static IOptions<SecretProtectionSettings> CreateOptions()
    {
        return Options.Create(new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8,
                9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24,
                25, 26, 27, 28, 29, 30, 31, 32
            })
        });
    }
}
