using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Pagination;
using Fliq.Contracts.MatchedProfile;
using Fliq.Domain.Entities.MatchedProfile;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class MatchProfileRepository : IMatchProfileRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public MatchProfileRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _dbContext = dbContext;
        }
        public void Add(MatchRequest matchProfile)
        {
            if (matchProfile.Id > 0)
            {
                _dbContext.Update(matchProfile);
            }
            else
            {
                _dbContext.Add(matchProfile);
            }
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<MatchRequestDto>> GetMatchListById(int userId, MatchListPagination matchListPagination)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = FilterListDynamicParams(userId, matchListPagination);

                var sql = "sPGetMatchedList";

                var result = connection.Query<dynamic>(sql, param: parameters, commandType: CommandType.StoredProcedure);
                var filteredItems = result.Select(p => new MatchRequestDto
                {
                    MatchInitiatorUserId = p.MatchInitiatorUserId,
                    Name = p.Name,
                    PictureUrl = p.PictureUrl
                }).ToList();
                return filteredItems;
            }
        }

        public MatchRequest? GetMatchProfileByUserId(int Id)
        {
            var matchProfile = _dbContext.MatchRequests.SingleOrDefault(p => p.UserId == Id);
            return matchProfile;
        }

        public MatchRequest? GetMatchProfileById(int Id)
        {
            var matchProfile = _dbContext.MatchRequests.SingleOrDefault(p => p.Id == Id);
            return matchProfile;
        }

        public void Update(MatchRequest request)
        {
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        private static DynamicParameters FilterListDynamicParams(int userId, MatchListPagination paginationRequest)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userId", userId);
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);
            return parameters;
        }
    }
}
