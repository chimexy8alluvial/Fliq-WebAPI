using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.MatchedProfile.Commands.MatchedList
{
    public class CreateMatchListCommandValidator : AbstractValidator<CreateMatchListCommand>
    {
        public CreateMatchListCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
