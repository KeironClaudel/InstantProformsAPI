using FluentValidation;

namespace InstantProforms.Application.Features.CompanyConfig.ReplaceLogo;

/// <summary>
/// Validates <see cref="ReplaceCompanyLogoCommand"/>.
/// </summary>
public sealed class ReplaceCompanyLogoCommandValidator : AbstractValidator<ReplaceCompanyLogoCommand>
{
    public ReplaceCompanyLogoCommandValidator()
    {
        RuleFor(x => x.LogoFile)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Logo file is required.")
            .Must(x => x.Length <= 2 * 1024 * 1024)
            .WithMessage("Max size is 2MB.")
            .Must(file =>
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return ext is ".png" or ".jpg" or ".jpeg" or ".webp";
            })
            .WithMessage("Invalid file type.");
    }
}