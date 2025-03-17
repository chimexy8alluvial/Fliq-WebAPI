
using FluentValidation;

namespace Fliq.Application.Prompts.Commands.AddSystemPrompt
{
    public class AddSystemPromptCommandValidator : AbstractValidator<AddSystemPromptCommand>
    {
        public AddSystemPromptCommandValidator()
        {
            RuleFor(x => x.QuestionText).NotEmpty().WithMessage("Valid category name is required");
        }
    }
}
