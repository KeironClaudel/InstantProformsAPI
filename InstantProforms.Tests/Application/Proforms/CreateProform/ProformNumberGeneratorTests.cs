using InstantProforms.Application.Features.Proforms.CreateProform;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.CreateProform;

public sealed class ProformNumberGeneratorTests
{
    [Fact]
    public void GenerateNextNumber_WhenNoPreviousNumber_StartsYearSequenceAt200()
    {
        var result = ProformNumberGenerator.GenerateNextNumber(null, 2026);

        Assert.Equal("C2026200", result);
    }

    [Fact]
    public void GenerateNextNumber_WhenSameYear_IncrementsSequence()
    {
        var result = ProformNumberGenerator.GenerateNextNumber("C2026200", 2026);

        Assert.Equal("C2026201", result);
    }

    [Fact]
    public void GenerateNextNumber_WhenYearChanges_ResetsSequenceAndAdvancesLetter()
    {
        var result = ProformNumberGenerator.GenerateNextNumber("C2026209", 2027);

        Assert.Equal("D2027200", result);
    }
}
