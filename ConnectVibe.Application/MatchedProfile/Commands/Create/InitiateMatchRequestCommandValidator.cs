using FluentValidation;

namespace Fliq.Application.MatchedProfile.Commands.Create
{
    public class InitiateMatchRequestCommandValidator : AbstractValidator<InitiateMatchRequestCommand>
    {
        public InitiateMatchRequestCommandValidator()
        {
            RuleFor(x => x.MatchReceiverUserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.MatchInitiatorUserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}