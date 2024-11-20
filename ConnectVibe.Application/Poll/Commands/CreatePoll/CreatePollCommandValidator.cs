using FluentValidation;

namespace Fliq.Application.Poll.Commands.CreatePoll
{
    public class CreatePollCommandValidator : AbstractValidator<CreatePollCommand>
    {
        public CreatePollCommandValidator()
        {
            RuleFor(x => x.EventId).GreaterThan(0).WithMessage("EventId must be greater than 0.");
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(x => x.Question).NotEmpty().WithMessage("Please enter a valid Question.");
        }
    }
}
