using FluentValidation;

namespace InstantProforms.Application.Features.Auth.RegisterCompany;

/// <summary>
/// Validates <see cref="RegisterCompanyCommand"/>.
/// </summary>
public sealed class RegisterCompanyCommandValidator : AbstractValidator<RegisterCompanyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterCompanyCommandValidator"/> class.
    /// </summary>
    public RegisterCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.CompanySlug)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CompanyEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CompanyEmail));

        RuleFor(x => x.CompanyPhone)
            .MaximumLength(50);

        RuleFor(x => x.TaxPercentage)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.CompanyAddress)
            .MaximumLength(300);

        RuleFor(x => x.CompanyWebsite)
            .MaximumLength(200);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LegalName)
            .MaximumLength(200);

        RuleFor(x => x.TermsAndConditions)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.PrimaryColor)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.SecondaryColor)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.AccentColor)
            .NotEmpty()
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

        RuleFor(x => x.LogoFile)
            .NotNull()
            .Must(file => file.Length > 0)
            .WithMessage("Logo file is required.")
            .Must(file => file.Length <= 2 * 1024 * 1024)
            .WithMessage("Logo file size must not exceed 2 MB.")
            .Must(file =>
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return extension is ".png" or ".jpg" or ".jpeg" or ".webp";
            })
            .WithMessage("Only PNG, JPG, JPEG, and WEBP logo files are allowed.");

        RuleFor(x => x.OwnerFullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.OwnerEmail)
            .NotEmpty()
            .MaximumLength(200)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
    }
}