using InstantProforms.Application.Features.Proforms.GetPagedProforms;
using InstantProforms.Domain.Enums;
using Xunit;

namespace InstantProforms.Tests.Application.Proforms.GetPagedProforms;

public sealed class GetPagedProformsQueryValidatorTests
{
    [Fact]
    public void Validate_Fails_WhenToDateIsBeforeFromDate()
    {
        var validator = new GetPagedProformsQueryValidator();

        var result = validator.Validate(new GetPagedProformsQuery(
            FromDate: new DateOnly(2026, 5, 10),
            ToDate: new DateOnly(2026, 5, 1)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ToDate");
    }

    [Fact]
    public void Validate_Succeeds_WithValidFilters()
    {
        var validator = new GetPagedProformsQueryValidator();

        var result = validator.Validate(new GetPagedProformsQuery(
            Page: 1,
            PageSize: 50,
            ClientName: "Eco",
            Status: ProformStatus.Accepted,
            FromDate: new DateOnly(2026, 5, 1),
            ToDate: new DateOnly(2026, 5, 31)));

        Assert.True(result.IsValid);
    }
}
