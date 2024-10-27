using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Prompts.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Prompts;
using MediatR;


namespace Fliq.Application.Prompts.Commands
{
    public record AddSystemPromptCommand(string QuestionText, int CategoryId) : IRequest<ErrorOr<AddSystemPromptResult>>;

    public class AddSystemPromptCommandHandler : IRequestHandler<AddSystemPromptCommand, ErrorOr<AddSystemPromptResult>>
    {
        private readonly IPromptQuestionRepository _questionRepository;
        private readonly IPromptCategoryRepository _categoryRepository;

        public AddSystemPromptCommandHandler(IPromptQuestionRepository questionRepository, IPromptCategoryRepository categoryRepository)
        {
            _questionRepository = questionRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ErrorOr<AddSystemPromptResult>> Handle(AddSystemPromptCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);

            if (category is null) return Errors.Prompts.CategoryNotFound;

            var promptQuestion = new PromptQuestion
            {
                QuestionText = request.QuestionText,
                IsSystemGenerated = true,
                PromptCategoryId = request.CategoryId
            };

            _questionRepository.AddQuestion(promptQuestion);

            return new AddSystemPromptResult(promptQuestion.Id, promptQuestion.QuestionText);
        }
    }
}
