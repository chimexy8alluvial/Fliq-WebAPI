using Azure;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using FluentValidation;

namespace Fliq.Application.Profile.Commands.Create
{
    public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
    {
        public CreateProfileCommandValidator()
        {
            RuleFor(x => x.DOB)
                .NotEmpty().WithMessage("Date of Birth is required.");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Gender is required.")
                .SetValidator(new GenderValidator());

            RuleFor(x => x.SexualOrientation)
            .NotNull().When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
            .WithMessage("Sexual Orientation is required for Dating or Friendship profile types.")
            .SetValidator(new SexualOrientationValidator());

            RuleFor(x => x.Religion)
                .NotNull().WithMessage("Religion is required.")
                .SetValidator(new ReligionValidator());

            RuleFor(x => x.Ethnicity)
                .NotNull().WithMessage("Ethnicity is required.")
                .SetValidator(new EthnicityValidator());

            RuleFor(x => x.Occupation)
           .NotNull().WithMessage("Occupation is required.")
           .SetValidator(new OccupationValidator());

            RuleFor(x => x.EducationStatus)
                .NotNull().WithMessage("Education Status is required.")
                .SetValidator(new EducationStatusValidator());

            RuleFor(x => x.HaveKids)
           .NotNull().When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
           .WithMessage("HaveKids is required for Dating or Friendship profile types.")
           .SetValidator(new HaveKidsValidator());

            RuleFor(x => x.WantKids)
           .NotNull().When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
           .WithMessage("WantKids is required for Dating or Friendship profile types.")
           .SetValidator(new WantKidsValidator());

            RuleFor(x => x.Photos)
           .NotNull().WithMessage("Photos are required.")
           .Must(photos => photos.Count == 1).WithMessage("Exactly 6 photos are required.")
           .ForEach(photo => photo.SetValidator(new ProfilePhotoDtoValidator()));

            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new LocationValidator());

            RuleFor(x => x.AllowNotifications)
                .NotNull().WithMessage("AllowNotifications is required.");

            RuleForEach(x => x.ProfileTypes).IsInEnum()
                .WithMessage("Invalid ProfileType value.");

            // Profile Description Rule
            RuleFor(x => x.ProfileDescription)
                .NotEmpty().When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
                .WithMessage("Profile description is required for Dating or Friendship profile types.");

            RuleFor(x => x.PromptResponses)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
                .WithMessage("Prompts should be provided for Dating or Friendship profile types")
                .ForEach(response => response.SetValidator(new PromptResponseValidator()))
                .When(x => x.PromptResponses != null);
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

    public class SexualOrientationValidator : AbstractValidator<SexualOrientation?>
    {
        public SexualOrientationValidator()
        {
            RuleFor(x => x)
            .NotNull().WithMessage("Sexual Orientation is required.")
            .Must(x => x != null && Enum.IsDefined(typeof(SexualOrientationType), x.SexualOrientationType))
            .WithMessage("Invalid SexualOrientationType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.")
                .When(x => x != null);
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

    public class HaveKidsValidator : AbstractValidator<HaveKids?>
    {
        public HaveKidsValidator()
        {
            RuleFor(x => x)
             .NotNull().WithMessage("Have Kids information is required.")
             .Must(x => x != null && Enum.IsDefined(typeof(HaveKidsType), x.HaveKidsType))
             .WithMessage("Invalid HaveKidsType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.")
                .When(x => x != null);
        }
    }

    public class WantKidsValidator : AbstractValidator<WantKids?>
    {
        public WantKidsValidator()
        {
            RuleFor(x => x)
             .NotNull().WithMessage("Want Kids information is required.")
             .Must(x => x != null && Enum.IsDefined(typeof(WantKidsType), x.WantKidsType))
             .WithMessage("Invalid WantKidsType value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.")
                .When(x => x != null);
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

    public class OccupationValidator : AbstractValidator<Occupation>
    {
        public OccupationValidator()
        {
            RuleFor(x => x.OccupationName)
                .NotEmpty().WithMessage("Occupation name is required.")
                .MaximumLength(100).WithMessage("Occupation name cannot be longer than 100 characters.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class EducationStatusValidator : AbstractValidator<EducationStatus>
    {
        public EducationStatusValidator()
        {
            RuleFor(x => x.EducationLevel)
                .IsInEnum().WithMessage("Invalid EducationLevel value.");

            RuleFor(x => x.IsVisible)
                .NotNull().WithMessage("IsVisible is required.");
        }
    }

    public class PromptResponseValidator : AbstractValidator<PromptResponseDto>
    {
        public PromptResponseValidator()
        {
            RuleFor(x => x.PromptQuestionId)
                .NotEmpty().WithMessage("QuestionId is required.");

            RuleFor(x => x)
                .Must(x => OnlyOneAnswerProvided(x))
                .WithMessage("Only one answer type (Text, Audio, or Video) can be provided per prompt.");

            RuleFor(x => x.TextResponse)
                .NotEmpty().When(x => !string.IsNullOrEmpty(x.TextResponse))
                .WithMessage("AnswerText cannot be empty if provided.")
                .MaximumLength(500).WithMessage("AnswerText cannot exceed 500 characters.");

            RuleFor(x => x.VoiceNote)
                .NotEmpty().When(x => x.VoiceNote != null)
                .WithMessage("Voice note file cannot be empty if provided.");

            RuleFor(x => x.VideoClip)
                .NotEmpty().When(x => x.VideoClip != null)
                .WithMessage("Video clip file cannot be empty if provided.");
        }

        private bool OnlyOneAnswerProvided(PromptResponseDto response)
        {
            int answerCount = 0;

            if (!string.IsNullOrEmpty(response.TextResponse)) answerCount++;
            if (response.VoiceNote != null) answerCount++;
            if (response.VideoClip != null) answerCount++;

            // Ensure only one answer is provided
            return answerCount == 1;
        }
    }

}