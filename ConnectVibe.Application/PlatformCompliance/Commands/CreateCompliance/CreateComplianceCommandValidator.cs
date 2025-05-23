using FluentValidation;

namespace Fliq.Application.PlatformCompliance.Commands.CreateCompliance
{
    public class CreateComplianceCommandValidator : AbstractValidator<CreateComplianceCommand>
    {
        public CreateComplianceCommandValidator()
        {
            RuleFor(x => x.ComplianceTypeId)
                .GreaterThan(0)
                .WithMessage("ComplianceTypeId must be greater than 0.");

            RuleFor(x => x.Language)
                .IsInEnum()
                .WithMessage("Invalid language specified.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required.")
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.VersionNumber)
                .NotEmpty()
                .WithMessage("Version number is required.")
                .MaximumLength(50)
                .WithMessage("Version number must not exceed 50 characters.");

            RuleFor(x => x.EffectiveDate)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("Effective date must be in the future.");
        }
    }
}
