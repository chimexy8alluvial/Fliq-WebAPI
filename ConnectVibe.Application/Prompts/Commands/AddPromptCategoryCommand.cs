using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
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

        public AddPromptCategoryCommandHandler(IPromptCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ErrorOr<AddPromptCategoryResult>> Handle(AddPromptCategoryCommand request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var category = new PromptCategory
            {
                CategoryName = request.CategoryName,
                IsSystemGenerated = true
            };

            var existingCategory = _categoryRepository.GetCategoryByName(category.CategoryName);
            if (existingCategory != null)
            {
                return Errors.Prompts.DuplicateCategory;
            }

            _categoryRepository.AddCategory(category);

            return new AddPromptCategoryResult(category.Id, category.CategoryName);
        }
    }
}
