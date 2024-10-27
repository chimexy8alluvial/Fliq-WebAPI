

using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptAnswerRepository
    {
        void Add(PromptAnswer promptAnswer);
    }
}
