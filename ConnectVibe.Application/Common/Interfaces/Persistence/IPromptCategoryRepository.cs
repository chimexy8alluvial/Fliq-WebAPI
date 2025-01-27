using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptCategoryRepository
    {
        void AddCategory(PromptCategory category);
        PromptCategory? GetCategoryById(int categoryId);
        PromptCategory? GetCategoryByName(string categoryName);
        IEnumerable<PromptCategory> GetAllPromptCategories();
    }
}
