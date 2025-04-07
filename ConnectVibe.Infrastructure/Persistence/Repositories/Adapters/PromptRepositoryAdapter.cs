using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;

namespace Fliq.Infrastructure.Persistence.Repositories.Adapters
{
    public class PromptRepositoryAdapter : IGenericRepository<PromptQuestion>
    {
        private readonly IPromptQuestionRepository _promptQuestionRepository;

        public PromptRepositoryAdapter(IPromptQuestionRepository promptQuestionRepository)
        {
            _promptQuestionRepository = promptQuestionRepository;
        }

        public async Task<PromptQuestion?> GetByIdAsync(int id)
        {
            await Task.CompletedTask;
            return _promptQuestionRepository.GetQuestionByIdAsync(id);
        }

        public async Task UpdateAsync(PromptQuestion prompts)
        {
            await Task.CompletedTask;
            _promptQuestionRepository.AddQuestion(prompts);
        }
    }
}
