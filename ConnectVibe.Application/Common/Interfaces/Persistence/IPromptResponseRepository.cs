

using Fliq.Domain.Entities.Prompts;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IPromptResponseRepository
    {
        void Add(PromptResponse promptResponse);
    }
}
