using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PromptQuestionRepository : IPromptQuestionRepository
    {
        private readonly FliqDbContext _dbContext;

        public PromptQuestionRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddQuestion(PromptQuestion question)
        {

            if (question.Id > 0)
            {
                _dbContext.Update(question);
            }
            else
            {
                _dbContext.Add(question);
            }
            _dbContext.SaveChanges();
        }

        public Task<PromptQuestion> GetQuestionByIdAsync(int questionId)
        {
            throw new NotImplementedException();
        }
    }
}
