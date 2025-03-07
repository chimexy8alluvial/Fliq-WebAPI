
using FluentValidation;

namespace Fliq.Application.DatingEnvironment.Commands.BlindDateCategory
{
    public class AddBlindDateCategoryCommandValidator : AbstractValidator<AddBlindDateCategoryCommand>
    {
        public AddBlindDateCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty()
                .WithMessage("Category name is required");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MinimumLength(10)
                .WithMessage("Description must be at least 10 characters long")
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters")
                .Matches(@"^(?!.*(.)\1{5,})[a-zA-Z0-9\s.,!?'-]+$")
                .WithMessage("Description contains excessive repetition or invalid characters.");
        }
    }
}
