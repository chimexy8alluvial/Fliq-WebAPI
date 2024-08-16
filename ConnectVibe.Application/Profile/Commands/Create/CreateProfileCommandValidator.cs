using ConnectVibe.Contracts.Profile;
using ConnectVibe.Domain.Entities.Profile;
using FluentValidation;

namespace ConnectVibe.Application.Profile.Commands.Create
{
    public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
    {
        public CreateProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.DOB)
                .NotEmpty().WithMessage("Date of Birth is required.");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender is required.")
                .SetValidator(new GenderValidator());

            RuleFor(x => x.SexualOrientation)
                .NotNull().WithMessage("Sexual Orientation is required.")
                .SetValidator(new SexualOrientationValidator());

            RuleFor(x => x.Religion)
                .NotNull().WithMessage("Religion is required.")
                .SetValidator(new ReligionValidator());

            RuleFor(x => x.Ethnicity)
                .NotNull().WithMessage("Ethnicity is required.")
                .SetValidator(new EthnicityValidator());

            RuleFor(x => x.HaveKids)
                .NotNull().WithMessage("HaveKids is required.")
                .SetValidator(new HaveKidsValidator());

            RuleFor(x => x.WantKids)
                .NotNull().WithMessage("WantKids is required.")
                .SetValidator(new WantKidsValidator());

            RuleFor(x => x.Passions)
                .NotEmpty().WithMessage("Passions cannot be empty.");

            RuleForEach(x => x.Photos)
                .SetValidator(new ProfilePhotoDtoValidator());

            RuleFor(x => x.ShareLocation)
                .NotNull().WithMessage("ShareLocation is required.");

            RuleFor(x => x.AllowNotifications)
                .NotNull().WithMessage("AllowNotifications is required.");
        }
    }

    // Validators for nested DTOs
    public class GenderValidator : AbstractValidator<Gender>
    {
        public GenderValidator()
        {
            RuleFor(x => x.GenderType)
                .IsInEnum().WithMessage("Invalid GenderType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class SexualOrientationValidator : AbstractValidator<SexualOrientation>
    {
        public SexualOrientationValidator()
        {
            RuleFor(x => x.SexualOrientationType)
                .IsInEnum().WithMessage("Invalid SexualOrientationType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class ReligionValidator : AbstractValidator<Religion>
    {
        public ReligionValidator()
        {
            RuleFor(x => x.ReligionType)
                .IsInEnum().WithMessage("Invalid ReligionType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class EthnicityValidator : AbstractValidator<Ethnicity>
    {
        public EthnicityValidator()
        {
            RuleFor(x => x.EthnicityType)
                .IsInEnum().WithMessage("Invalid EthnicityType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class HaveKidsValidator : AbstractValidator<HaveKids>
    {
        public HaveKidsValidator()
        {
            RuleFor(x => x.HaveKidsType)
                .IsInEnum().WithMessage("Invalid HaveKidsType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class WantKidsValidator : AbstractValidator<WantKids>
    {
        public WantKidsValidator()
        {
            RuleFor(x => x.WantKidsType)
                .IsInEnum().WithMessage("Invalid WantKidsType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class ProfilePhotoDtoValidator : AbstractValidator<ProfilePhotoDto>
    {
        public ProfilePhotoDtoValidator()
        {
            RuleFor(x => x.Caption)
                .NotEmpty().WithMessage("Caption is required.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("ImageFile is required.");
        }
    }
}