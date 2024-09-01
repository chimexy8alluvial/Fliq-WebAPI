using ConnectVibe.Application.Profile.Common;
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

            RuleFor(x => x.Photos)
           .NotNull().WithMessage("Photos are required.")
           .Must(photos => photos.Count >= 1).WithMessage("Exactly 6 photos are required.")
           .ForEach(photo => photo.SetValidator(new ProfilePhotoDtoValidator()));

            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new LocationValidator());

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

    public class ProfilePhotoDtoValidator : AbstractValidator<ProfilePhotoMapped>
    {
        public ProfilePhotoDtoValidator()
        {
            RuleFor(x => x.Caption)
                .NotEmpty().WithMessage("Caption is required.");

            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("ImageFile is required.");
        }
    }

    public class LocationValidator : AbstractValidator<Location>
    {
        public LocationValidator()
        {
            // Latitude should be between -90 and 90
            RuleFor(location => location.Lat)
                .InclusiveBetween(-90.0, 90.0)
                .WithMessage("Latitude must be between -90 and 90 degrees.");

            // Longitude should be between -180 and 180
            RuleFor(location => location.Lng)
                .InclusiveBetween(-180.0, 180.0)
                .WithMessage("Longitude must be between -180 and 180 degrees.");

            // IsVisible should be a boolean, but no specific rule needed unless you want to ensure it's true/false
            RuleFor(location => location.IsVisible)
                .NotNull()
                .WithMessage("IsVisible must be specified.");
        }
    }
}
