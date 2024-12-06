using FluentValidation;

namespace Fliq.Application.Notifications.Commands.DeviceRegistration
{
    public class SaveDeviceTokenCommandValidator : AbstractValidator<SaveDeviceTokenCommand>
    {
        public SaveDeviceTokenCommandValidator()
        {
            RuleFor(x => x.UserId).NotNull().WithMessage("User Id is required for token registration request");
            RuleFor(x => x.DeviceToken).NotEmpty().WithMessage("Device Token is required for token registration");
        }
    }
}
