using FluentValidation;

namespace ConnectVibe.Application.Authentication.Commands.PasswordReset
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordValidator()
        {
           
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");;
        }
    }
}
