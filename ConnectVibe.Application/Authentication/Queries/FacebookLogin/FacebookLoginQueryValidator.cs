using FluentValidation;

namespace ConnectVibe.Application.Authentication.Queries.FacebookLogin
{
    public class FacebookLoginQueryValidator : AbstractValidator<FacebookLoginQuery>
    {
        public FacebookLoginQueryValidator()
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage("Code is required");
        }
    }
}