using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common.UnifiedSearch;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SearchRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        
        public async Task<UnifiedSearchResult> SearchAcrossEntitiesAsync(string searchTerm)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = new UnifiedSearchResult
            {
                Users = new List<UserSearchResult>(),
                Events = new List<EventSearchResult>(),
            };

            using var multi = await connection.QueryMultipleAsync(
                "sp_SearchAcrossEntities",
                new { SearchTerm = searchTerm },
                commandType: CommandType.StoredProcedure);

            result.Users = (await multi.ReadAsync<UserSearchResult>()).ToList();
            result.Events = (await multi.ReadAsync<EventSearchResult>()).ToList();

            return result;
        }
    }
}
