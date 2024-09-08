using ConnectVibe.Application.Profile.Commands.Create;
using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event;
using FluentValidation;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommandvalidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandvalidator()
        {
            //RuleFor(x => x.Docs)
            //    .NotNull().WithMessage("Document is required.");
            RuleFor(x => x.eventTitle).NotEmpty().WithMessage("Event title must not be empty!.");
            RuleFor(x => x.eventDescription).NotEmpty().WithMessage("Event Description must not be empty!.");
            RuleFor(x => x.Id)
               .GreaterThan(0).WithMessage("Id must be greater than 0.");
            RuleFor(x => x.UserId)
              .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(x => x.EventType).NotEmpty().WithMessage("Event Type must not be empty!");
            RuleFor(x => x.startDate).NotEmpty();
            RuleFor(x => x.endDate).NotEmpty();
            //RuleFor(x => x.timeZone).NotEmpty();
            RuleFor(x => x.capacity)
               .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
            RuleFor(x => x.Location)
               .NotNull().WithMessage("Location is required.")
               .SetValidator(new LocationValidator());
        }
     }
}
