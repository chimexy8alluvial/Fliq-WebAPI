using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Poll.Commands.Voting
{
    public class VoteCommandValidator : AbstractValidator<VoteCommand>
    {
        public VoteCommandValidator() 
        {
            RuleFor(x => x.EventId).GreaterThan(0).WithMessage("EventId must be greater than 0.");
            RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(x => x.Question).NotEmpty().WithMessage("Please enter a valid Question.");
        }
    }
}
