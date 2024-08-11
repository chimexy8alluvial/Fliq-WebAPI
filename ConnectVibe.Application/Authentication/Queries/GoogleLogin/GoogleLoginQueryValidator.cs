using FluentValidation;

namespace ConnectVibe.Application.Authentication.Queries.GoogleLogin
{
    public class GoogleLoginQueryValidator : AbstractValidator<GoogleLoginQuery>
    {
        public GoogleLoginQueryValidator()
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage("Code is required");
        }
    }
}