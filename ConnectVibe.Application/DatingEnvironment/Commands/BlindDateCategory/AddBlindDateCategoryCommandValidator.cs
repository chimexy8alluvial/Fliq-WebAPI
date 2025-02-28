
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
        }
    }
}
