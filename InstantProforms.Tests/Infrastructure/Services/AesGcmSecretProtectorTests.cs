using InstantProforms.Application.Common.Models;
using InstantProforms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace InstantProforms.Tests.Infrastructure.Services;

public sealed class AesGcmSecretProtectorTests
{
    [Fact]
    public void ProtectAndUnprotect_RoundTripsOriginalValue()
    {
        var protector = CreateProtector();

        var protectedValue = protector.Protect("re_secret_key_value");
        var plaintext = protector.Unprotect(protectedValue);

        Assert.NotEqual("re_secret_key_value", protectedValue);
        Assert.Equal("re_secret_key_value", plaintext);
    }

    [Fact]
    public void Constructor_WithInvalidKeyLength_Throws()
    {
        var options = Options.Create(new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 })
        });

        var exception = Assert.Throws<InvalidOperationException>(() => new AesGcmSecretProtector(options));
        Assert.Contains("32 bytes", exception.Message);
    }

    private static AesGcmSecretProtector CreateProtector()
    {
        return new AesGcmSecretProtector(Options.Create(new SecretProtectionSettings
        {
            MasterKey = Convert.ToBase64String(new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8,
                9, 10, 11, 12, 13, 14, 15, 16,
                17, 18, 19, 20, 21, 22, 23, 24,
                25, 26, 27, 28, 29, 30, 31, 32
            })
        }));
    }
}
