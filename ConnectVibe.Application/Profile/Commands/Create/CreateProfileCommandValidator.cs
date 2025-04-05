using Fliq.Application.Profile.Common;
using Fliq.Contracts.Prompts;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;
using FluentValidation;

namespace Fliq.Application.Profile.Commands.Create
{
    public class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
    {
        public CreateProfileCommandValidator()
        {
            RuleFor(x => x.CurrentSection)
                .NotNull().WithMessage("CurrentSection is required.")
                .IsInEnum().WithMessage("Invalid CurrentSection value.");

            When(x => x.CurrentSection == ProfileSection.BasicInfo, () =>
            {
                RuleFor(x => x.DOB)
                    .NotEmpty().WithMessage("Date of Birth is required.");

                RuleFor(x => x.GenderId)
                    .GreaterThan(0).WithMessage("Gender is required.");

                RuleFor(x => x.SexualOrientationId)
                    .GreaterThan(0).When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
                    .WithMessage("Sexual Orientation is required for Dating or Friendship profile types.");


                RuleFor(x => x.ReligionId)
                    .GreaterThan(0)
                    .WithMessage("Religion is required.");

                RuleFor(x => x.EthnicityId)
                     .GreaterThan(0).WithMessage("Ethnicity is required.");
                    

                RuleFor(x => x.OccupationId)
                    .GreaterThan(0).WithMessage("Occupation is required.");

                RuleFor(x => x.EducationStatusId)
                    .GreaterThan(0).WithMessage("Education Status is required.");

                RuleFor(x => x.HaveKidsId)
                    .GreaterThan(0).When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
                    .WithMessage("HaveKids is required for Dating or Friendship profile types.");

                RuleFor(x => x.WantKidsId)
                    .GreaterThan(0).When(x => x.ProfileTypes.Any(pt => pt == ProfileType.Dating || pt == ProfileType.Friendship))
                    .WithMessage("WantKids is required for Dating or Friendship profile types.");
            });

            When(x => x.CurrentSection == ProfileSection.Photos, () =>
            {
                RuleFor(x => x.Photos)
                    .NotNull().WithMessage("Photos are required.")
                    .Must(photos => photos.Count >= 1).WithMessage("At least one Photo is required.")
                    .ForEach(photo => photo.SetValidator(new ProfilePhotoDtoValidator()));
            });

            When(x => x.CurrentSection == ProfileSection.Location, () =>
            {
                RuleFor(x => x.Location)
                    .NotNull().WithMessage("Location is required.")
                    .SetValidator(new LocationValidator());

                RuleFor(x => x.LocationDetail)
                    .NotNull().WithMessage("Location details are required.");
            });

            When(x => x.CurrentSection == ProfileSection.Interests, () =>
            {
                RuleFor(x => x.Passions)
                    .NotNull().WithMessage("Passions are required.")
                    .Must(p => p.Count > 0).WithMessage("At least one passion is required.");
            });

            When(x => x.CurrentSection == ProfileSection.Preferences, () =>
            {
                RuleForEach(x => x.ProfileTypes)
                    .IsInEnum()
                    .WithMessage("Invalid ProfileType value.");
            });

            When(x => x.CurrentSection == ProfileSection.Prompts, () =>
            {
                RuleFor(x => x.PromptResponses)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Prompts should be provided")
                    .ForEach(response => response.SetValidator(new PromptResponseValidator()));
            });

            RuleFor(x => x.AllowNotifications)
                .NotNull().WithMessage("AllowNotifications is required.");
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