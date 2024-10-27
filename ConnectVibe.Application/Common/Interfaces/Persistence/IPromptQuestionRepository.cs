

using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptQuestionRepository
    {
        void AddQuestion(PromptQuestion question);
        Task<PromptQuestion> GetQuestionByIdAsync(int questionId);
    }
}
