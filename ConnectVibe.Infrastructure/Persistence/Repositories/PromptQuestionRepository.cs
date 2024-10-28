using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Prompts;
using Microsoft.AspNetCore.Connections;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class PromptQuestionRepository : IPromptQuestionRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public PromptQuestionRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
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

        public PromptQuestion? GetQuestionByIdAsync(int questionId)
        {
            var question =  _dbContext.PromptQuestions.SingleOrDefault(q => q.Id == questionId);
            return question;
        }

        public IEnumerable<PromptQuestion> GetPromptQuestionsByCategory(int categoryId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
               var parameters = new DynamicParameters();
               parameters.Add("@CategoryId", categoryId);

               var promptQuestions = connection.Query<PromptQuestion>("sp_GetPromptQuestionsByCategory", param: parameters, commandType: CommandType.StoredProcedure);

               return promptQuestions;
            }
        }

    }
}
