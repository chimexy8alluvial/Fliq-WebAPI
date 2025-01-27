

namespace Fliq.Contracts.Prompts
{
    public record AddPromptCategoryResponse(int CategoryId, string CategoryName);

    public record GetPromptCategoryResponse(int CategoryId, string CategoryName);

}
