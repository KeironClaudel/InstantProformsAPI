using FluentValidation;

namespace InstantProforms.Application.Features.CompanyConfig.UpsertCompanySettings;

/// <summary>
/// Validates <see cref="UpsertCompanySettingsCommand"/>.
/// </summary>
public sealed class UpsertCompanySettingsCommandValidator : AbstractValidator<UpsertCompanySettingsCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpsertCompanySettingsCommandValidator"/> class.
    /// </summary>
    public UpsertCompanySettingsCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LegalName)
            .MaximumLength(200);

        RuleFor(x => x.Website)
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(300);

        RuleFor(x => x.TermsAndConditions)
            .MaximumLength(4000);

        RuleFor(x => x.LogoFileName)
            .MaximumLength(255);

        RuleFor(x => x.PrimaryColor)
            .MaximumLength(20);

        RuleFor(x => x.SecondaryColor)
            .MaximumLength(20);

        RuleFor(x => x.TaxPercentage)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.AccentColor)
            .MaximumLength(20);

        RuleFor(x => x.ProformPrefix)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.CurrencySymbol)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(x => x.TaxLabel)
            .NotEmpty()
            .MaximumLength(50);
    }
}