using FluentValidation;

namespace ConnectVibe.Application.Authentication.Commands.ValidateOTP
{
    public class ValidateOTPCommandValidator : AbstractValidator<ValidateOTPCommand>
    {
        public ValidateOTPCommandValidator()
        {
            RuleFor(x => x.Otp).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
        }

    }
}
