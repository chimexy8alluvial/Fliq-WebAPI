using FluentValidation;

namespace Fliq.Application.MatchedProfile.Commands.Create
{
    public class CreateMatchedProfileCommandValidator : AbstractValidator<CreateMatchProfileCommand>
    {
        public CreateMatchedProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.MatchInitiatorUserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
