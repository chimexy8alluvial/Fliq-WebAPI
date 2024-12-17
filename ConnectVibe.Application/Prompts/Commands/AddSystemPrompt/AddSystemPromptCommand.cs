using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Prompts.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Prompts;
using MediatR;


namespace Fliq.Application.Prompts.Commands.AddSystemPrompt
{
    public record AddSystemPromptCommand(string QuestionText, int CategoryId) : IRequest<ErrorOr<AddSystemPromptResult>>;

    public class AddSystemPromptCommandHandler : IRequestHandler<AddSystemPromptCommand, ErrorOr<AddSystemPromptResult>>
    {
        private readonly IPromptQuestionRepository _questionRepository;
        private readonly IPromptCategoryRepository _categoryRepository;
        private readonly ILoggerManager _loggerManager;

        public AddSystemPromptCommandHandler(IPromptQuestionRepository questionRepository, IPromptCategoryRepository categoryRepository, ILoggerManager loggerManager)
        {
            _questionRepository = questionRepository;
            _categoryRepository = categoryRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<AddSystemPromptResult>> Handle(AddSystemPromptCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _loggerManager.LogInfo($"Starting prompt question creation process for Category ID: {request.CategoryId}");

            var category = _categoryRepository.GetCategoryById(request.CategoryId);

            if (category is null)
            {
                _loggerManager.LogWarn($"Category not found for Category ID: {request.CategoryId}. Aborting prompt question creation.");
                return Errors.Prompts.CategoryNotFound;
            }

            var QuestionExist = _questionRepository.QuestionExistInCategory(request.CategoryId, request.QuestionText);
            if (QuestionExist)
            {
                _loggerManager.LogWarn($"{request.QuestionText} Question already exist for Category ID: {request.CategoryId}. Aborting prompt question creation.");
                return Errors.Prompts.DuplicateCategoryQuestion;
            }
            var promptQuestion = new PromptQuestion
            {
                QuestionText = request.QuestionText,
                IsSystemGenerated = true,
                PromptCategoryId = request.CategoryId
            };

            _questionRepository.AddQuestion(promptQuestion);
            _loggerManager.LogInfo($"Successfully added prompt question: '{promptQuestion.QuestionText}' with ID: {promptQuestion.Id} to Category ID: {request.CategoryId}");

            return new AddSystemPromptResult(promptQuestion.Id, promptQuestion.PromptCategoryId, promptQuestion.QuestionText);
        }
    }
}
