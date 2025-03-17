

using ErrorOr;
using Fliq.Application.Profile.Commands.Create;
using FluentValidation;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDates
{
    public class CreateBlindDateCommandValidator : AbstractValidator<CreateBlindDateCommand>
    {
        public CreateBlindDateCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title is required");

            RuleFor(x => x.CreatedByUserId)
             .GreaterThan(0)
             .WithMessage("CategoryId must be a valid ID.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be a valid ID.");

            RuleFor(x => x.StartDateTime)
             .NotNull()
             .WithMessage("Start time required.");

            RuleFor(x => x.Location)
                .NotNull().WithMessage("Location is required.")
                .SetValidator(new LocationValidator());

            RuleFor(x => x.LocationDetail)
                .NotNull().WithMessage("Location details are required.");
        }
    }
}
