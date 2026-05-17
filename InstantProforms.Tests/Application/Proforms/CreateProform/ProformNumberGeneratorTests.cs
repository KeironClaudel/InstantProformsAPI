using InstantProforms.Application.Features.Proforms.CreateProform;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.CreateProform;

public sealed class ProformNumberGeneratorTests
{
    [Fact]
    public void GenerateNextNumber_WhenNoPreviousNumber_StartsYearSequenceAt200()
    {
        var result = ProformNumberGenerator.GenerateNextNumber(null, "C", 2026, 2026);

        Assert.Equal("C2026200", result);
    }

    [Fact]
    public void GenerateNextNumber_WhenSameYear_IncrementsSequence()
    {
        var result = ProformNumberGenerator.GenerateNextNumber("C2026200", "C", 2026, 2026);

        Assert.Equal("C2026201", result);
    }

    [Fact]
    public void GenerateNextNumber_WhenYearChanges_ResetsSequenceAndAdvancesLetter()
    {
        var result = ProformNumberGenerator.GenerateNextNumber("C2026209", "C", 2026, 2027);

        Assert.Equal("D2027200", result);
    }

    [Fact]
    public void GenerateNextNumber_WhenPrefixHasMultipleLetters_AdvancesAlphabeticallyFromConfiguredBase()
    {
        var result = ProformNumberGenerator.GenerateNextNumber("ECO2026205", "ECO", 2026, 2027);

        Assert.Equal("ECP2027200", result);
    }
}
