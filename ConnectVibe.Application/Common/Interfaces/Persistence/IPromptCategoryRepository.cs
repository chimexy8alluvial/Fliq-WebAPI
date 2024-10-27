using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptCategoryRepository
    {
        void AddCategory(PromptCategory category);
        Task<PromptCategory> GetCategoryByIdAsync(int category);
        Task<PromptCategory> GetCategoryByName(string categoryName);
    }
}
