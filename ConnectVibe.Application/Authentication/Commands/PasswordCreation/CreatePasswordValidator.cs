using FluentValidation;

namespace Fliq.Application.Authentication.Commands.PasswordCreation
{
    public class CreatePasswordValidator : AbstractValidator<CreatePasswordCommand>
    {
        public CreatePasswordValidator() 
        {
            RuleFor(x => x.Id).NotEmpty().GreaterThan(0);
            RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x=>x.NewPassword);
            RuleFor(x => x.NewPassword).NotEmpty()
                 .MinimumLength(10).WithMessage("Password must be at least 10 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character."); ;
        }
    }
}
