using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.MatchedProfile.Commands.RejectMatch
{
    public class RejectMatchRequestCommandValidator : AbstractValidator<RejectMatchRequestCommand>
    {
        public RejectMatchRequestCommandValidator() 
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
