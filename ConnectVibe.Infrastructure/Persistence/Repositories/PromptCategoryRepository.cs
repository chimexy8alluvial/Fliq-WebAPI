using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;


namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PromptCategoryRepository : IPromptCategoryRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        public PromptCategoryRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void AddCategory(PromptCategory category)
        {

            if (category.Id > 0)
            {
                _dbContext.Update(category);
            }
            else
            {
                _dbContext.Add(category);
            }
            _dbContext.SaveChanges();
        }

        public Task<PromptCategory> GetCategoryByIdAsync(int category)
        {
            throw new NotImplementedException();
        }
    }
}
