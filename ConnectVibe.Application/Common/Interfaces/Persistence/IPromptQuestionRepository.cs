

using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptQuestionRepository
    {
        void AddQuestion(PromptQuestion question);
        PromptQuestion? GetQuestionByIdAsync(int questionId);
        IEnumerable<PromptQuestion> GetPromptQuestionsByCategory(int categoryId);
        bool QuestionExistInCategory(int categoryId, string QuestionText);
    }
}
