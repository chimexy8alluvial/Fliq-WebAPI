using FluentValidation;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommandvalidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandvalidator()
        {
            RuleFor(x => x.Docs)
                .NotNull().WithMessage("Document is required.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.EventId)
               .GreaterThan(0).WithMessage("UserId must be greater than 0.");
            RuleFor(x => x.EventType).NotEmpty().WithMessage("Event Title must not be empty!");
        }
    }
}
