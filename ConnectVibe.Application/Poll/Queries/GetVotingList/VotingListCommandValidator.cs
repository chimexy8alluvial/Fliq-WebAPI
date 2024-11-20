using FluentValidation;

namespace Fliq.Application.Poll.Queries.GetVotingList
{
    public class VotingListCommandValidator : AbstractValidator<VotingListCommand>
    {
        public VotingListCommandValidator() 
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
