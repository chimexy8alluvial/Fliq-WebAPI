

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PromptResponseRepository : IPromptResponseRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public PromptResponseRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(PromptResponse promptAnswer)
        {
            if (promptAnswer.Id > 0)
            {
                _dbContext.Update(promptAnswer);
            }
            else
            {
                _dbContext.Add(promptAnswer);
            }
            _dbContext.SaveChanges();
        }

    }
}
