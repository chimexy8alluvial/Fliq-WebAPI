using FluentValidation;

namespace ConnectVibe.Application.Authentication.Commands.ChangePassword
{
    public class ChangePasswordValidation : AbstractValidator<ChangePasswordQuery>
    {
        public ChangePasswordValidation()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.OldPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty()
                 .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character."); ;
        }
    }
}
