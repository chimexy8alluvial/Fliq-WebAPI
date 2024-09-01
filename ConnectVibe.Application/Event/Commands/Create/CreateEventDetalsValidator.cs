using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectVibe.Domain.Entities.Profile;
using ConnectVibe.Application.Profile.Commands.Create;
using FluentValidation;

namespace Fliq.Application.Event.Commands.Create
{
    public class CreateEventDetalsValidator : AbstractValidator<CreateEventDetailsCommand>
    {
        public CreateEventDetalsValidator()
        {
            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new LocationValidator());
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x => x.endDate).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Id)
               .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(x => x.eventTitle).NotEmpty().WithMessage("Event Title must not be empty!");
            RuleFor(x => x.eventDescription).NotEmpty().WithMessage("Event Description must not be empty!");
            RuleFor(x => x.timeZone).NotEmpty();
            RuleFor(x => x.capacity)
               .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
            //RuleFor(x => x.optional).NotEmpty();

        }
        
    }
    
}
