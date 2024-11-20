using FluentValidation;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public class GetMatchRequestListCommandValidator : AbstractValidator<GetMatchRequestListCommand>
    {
        public GetMatchRequestListCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
