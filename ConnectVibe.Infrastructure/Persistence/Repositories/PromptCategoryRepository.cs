using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;
using System.Data;


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

        public PromptCategory? GetCategoryById(int categoryId)
        {
            var category = _dbContext.PromptCategories.SingleOrDefault(c => c.Id == categoryId);
            return category;
        }

        public PromptCategory? GetCategoryByName(string categoryName)
        {
            var category = _dbContext.PromptCategories.SingleOrDefault(c => c.CategoryName.ToLower() == categoryName.ToLower());
            return category;
        }

        public IEnumerable<PromptCategory> GetAllPromptCategories()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var categories = connection.Query<PromptCategory>("sp_GetAllPromptCategories", commandType: CommandType.StoredProcedure);

                return categories;
            }
        }

    }
}
