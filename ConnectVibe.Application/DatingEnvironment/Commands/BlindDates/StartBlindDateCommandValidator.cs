using FluentValidation;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public class StartBlindDateCommandValidator : AbstractValidator<StartBlindDateCommand>
    {
        public StartBlindDateCommandValidator()
        {
            RuleFor(x => x.BlindDateId)
                .GreaterThan(0)
                .WithMessage("BlindDateId must be a valid ID.");

            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("UserId must be a valid ID.");
        }
    }
}
