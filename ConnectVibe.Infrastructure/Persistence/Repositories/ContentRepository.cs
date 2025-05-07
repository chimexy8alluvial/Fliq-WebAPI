using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Contracts.Contents;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ContentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ContentTypeCount>> GetAllContentCountsAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<ContentTypeCount>(
                    "sp_GetAllContentCount",
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> GetTotalContentCountAsync()
        {
            var counts = await GetAllContentCountsAsync();
            return counts.Sum(c => c.Count);
        }
    }
}
