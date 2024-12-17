
using FluentValidation;

namespace Fliq.Application.Prompts.Commands
{
    public class AddPromptCategoryCommandValidator : AbstractValidator<AddPromptCategoryCommand>
    {
        public AddPromptCategoryCommandValidator()
        {
            RuleFor(x => x.CategoryName).NotEmpty().WithMessage("Valid category name is required");
        }
    }
}
