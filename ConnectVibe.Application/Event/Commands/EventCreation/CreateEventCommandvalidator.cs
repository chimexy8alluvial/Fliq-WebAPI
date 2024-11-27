using Fliq.Application.Profile.Commands.Create;
using Fliq.Domain.Entities.Event;
using FluentValidation;

namespace Fliq.Application.Event.Commands.EventCreation
{
    public class CreateEventCommandvalidator : AbstractValidator<CreateEventCommand>
    {
        public CreateEventCommandvalidator()
        {
            RuleFor(x => x.MediaDocuments)
                .NotNull().WithMessage("Document is required.");

            RuleFor(x => x.EventTitle).NotEmpty().WithMessage("Event title must not be empty!.");

            RuleFor(x => x.EventDescription).NotEmpty().WithMessage("Event Description must not be empty!.");

            //RuleFor(x => x.Id)
            //   .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.UserId)
              .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.EventType).IsInEnum().WithMessage("Event Type is required!");

            RuleFor(x => x.StartDate).NotEmpty();

            RuleFor(x => x.EndDate).NotEmpty();

            RuleFor(x => x.Capacity)
               .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

            RuleFor(x => x.Location)
               .NotNull().WithMessage("Location is required.")
               .SetValidator(new LocationValidator());

            RuleFor(x => x.MinAge).NotEmpty().WithMessage("Min Age must not be empty!");
            RuleFor(x => x.MaxAge).NotEmpty().WithMessage("Max Age must not be empty!");

            RuleFor(x => x.SponsoredEventDetail)
                .NotNull().WithMessage("Sponsored Event Detail is required.")
                .SetValidator(new SponsoredEventDetailValidator());

            RuleFor(x => x.EventCriteria)
                .NotNull().WithMessage("Event Criteria is required.")
                .SetValidator(new EventCriteriaValidator());

            RuleFor(x => x.Tickets)
                .NotNull().WithMessage("Ticket type is required.")
                .Must(Tickets => Tickets.Count >= 1).WithMessage("You can add more tickets!")
                .ForEach(Ticket => Ticket.SetValidator(new TicketValidator()));
        }
    }

    // Sponsored Event Detail Validator
    public class SponsoredEventDetailValidator : AbstractValidator<SponsoredEventDetail>
    {
        public SponsoredEventDetailValidator()
        {
            // Business Name
            RuleFor(x => x.BusinessName)
                .NotEmpty().WithMessage("Business Name is required.");
            // Business Address
            RuleFor(x => x.BusinessAddress)
                .NotEmpty().WithMessage("Business Address is required.");
            // Business Type
            RuleFor(x => x.BusinessType)
                .NotEmpty().WithMessage("Business Type list must not be empty.");
            // Sponsoring Budget
            RuleFor(x => x.SponsoringPlan)
                .NotNull().WithMessage("Pick one of the options for Sponsoring Plan.");
            // Target Audience
            RuleFor(x => x.TargetAudienceType)
                .NotEmpty().WithMessage("Target Audience Type is required.");
            // Number of Invitee

            // Budget
            RuleFor(x => x.Budget)
                .NotEmpty().WithMessage("Budget is required.");
            // Duration of Sponsorship
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Startdate of Sponsorship is required.");
            RuleFor(x => x.EndDate)
               .NotEmpty().WithMessage("Enddate of Sponsorship is required.");
            RuleFor(x => x.PreferedLevelOfInvolvement)
                .NotEmpty().WithMessage("Preferred Level of Involvement is required.");
        }
    }

    // Event Criteria Validator
    public class EventCriteriaValidator : AbstractValidator<EventCriteria>
    {
        public EventCriteriaValidator()
        {
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid Gender Type value.");

            RuleFor(x => x.EventType)
                .IsInEnum().WithMessage("Invalid Event Type value.");

            //RuleFor(x => x.Race)
            //    .NotEmpty().WithMessage("Race group is required.");
        }
    }

    //Ticket Validator for Payment Details
    public class TicketValidator : AbstractValidator<Ticket>
    {
        public TicketValidator()
        {
            RuleFor(x => x.TicketName)
                .NotEmpty().WithMessage("Ticket Name is required.");

            RuleFor(x => x.TicketDescription)
                .NotEmpty().WithMessage("Ticket Description is required.");

            RuleFor(x => x.TicketType)
                .NotEmpty().WithMessage("Ticket Type is required.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.");

            RuleFor(x => x.Amount)
                .NotNull().WithMessage("Amount is required.");
        }
    }
}