using FluentValidation;

namespace Fliq.Application.MatchedProfile.Commands.ApprovedMatchedList
{
    public class GetApprovedMatchListValidatorCommand : AbstractValidator<GetApprovedMatchListCommand>
    {
        public GetApprovedMatchListValidatorCommand()
        {
            RuleFor(x => x.UserId)
               .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
