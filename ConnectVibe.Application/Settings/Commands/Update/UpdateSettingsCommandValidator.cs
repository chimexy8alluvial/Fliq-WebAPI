using Fliq.Domain.Entities.Settings;
using FluentValidation;

namespace Fliq.Application.Settings.Commands.Update
{
    public class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
    {
        public UpdateSettingsCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.ScreenMode)
                .IsInEnum().WithMessage("Invalid screen mode.");

            RuleFor(x => x.Language)
                .NotEmpty().WithMessage("Language is required.")
                .MaximumLength(50).WithMessage("Language cannot exceed 50 characters.");

            RuleForEach(x => x.NotificationPreferences)
                .SetValidator(new NotificationPreferenceValidator());
        }
    }

    public class NotificationPreferenceValidator : AbstractValidator<NotificationPreference>
    {
        public NotificationPreferenceValidator()
        {
            RuleFor(x => x.Context)
                .NotEmpty().WithMessage("Notification context is required.")
                .MaximumLength(100).WithMessage("Notification context cannot exceed 100 characters.");

            RuleFor(x => x.PushNotification)
                .NotNull().WithMessage("PushNotification must be specified.");

            RuleFor(x => x.InAppNotification)
                .NotNull().WithMessage("InAppNotification must be specified.");
        }
    }
}