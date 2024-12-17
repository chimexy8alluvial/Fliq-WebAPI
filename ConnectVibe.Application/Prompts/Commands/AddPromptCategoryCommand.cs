using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Prompts.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Prompts;
using MediatR;


namespace Fliq.Application.Prompts.Commands
{
    public record AddPromptCategoryCommand(string CategoryName) : IRequest<ErrorOr<AddPromptCategoryResult>>;


    public class AddPromptCategoryCommandHandler : IRequestHandler<AddPromptCategoryCommand, ErrorOr<AddPromptCategoryResult>>
    {
        private readonly IPromptCategoryRepository _categoryRepository;
        private readonly ILoggerManager _loggerManager;

        public AddPromptCategoryCommandHandler(IPromptCategoryRepository categoryRepository, ILoggerManager loggerManager)
        {
            _categoryRepository = categoryRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<AddPromptCategoryResult>> Handle(AddPromptCategoryCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _loggerManager.LogInfo($"Starting category creation process for category name: {request.CategoryName}");

            var existingCategory = _categoryRepository.GetCategoryByName(request.CategoryName);
            if (existingCategory != null)
            {
                _loggerManager.LogWarn($"Duplicate category detected: {request.CategoryName}. Aborting creation.");
                return Errors.Prompts.DuplicateCategory;
            }

            //Create new prompt category object
            var category = new PromptCategory
            {
                CategoryName = request.CategoryName,
                IsSystemGenerated = true
            };
            _categoryRepository.AddCategory(category);
            _loggerManager.LogInfo($"Successfully added new category: {category.CategoryName} with ID: {category.Id}");

            return new AddPromptCategoryResult(category.Id, category.CategoryName);
        }
    }
}
