using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.UserFeatureActivities;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class UserFeatureActivityRepository : IUserFeatureActivityRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public UserFeatureActivityRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(UserFeatureActivity userActivity)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_AddOrUpdateUserFeatureActivity";
                var parameters = new DynamicParameters();

                parameters.Add("@Id", userActivity.Id);
                parameters.Add("@UserId", userActivity.UserId);
                parameters.Add("@Feature", userActivity.Feature);
                parameters.Add("@LastActiveAt", userActivity.LastActiveAt);
                parameters.Add("@IsDeleted", false);
                parameters.Add("@DateCreated", userActivity.DateCreated);

                await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<UserFeatureActivity?> GetUserFeatureActivityAsync(int userId, string feature)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_GetUserFeatureActivity";
                var parameters = new { UserId = userId, Feature = feature };

                return await connection.QueryFirstOrDefaultAsync<UserFeatureActivity>(sql, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<List<UserFeatureActivity>> GetInactiveFeatureUsersAsync(DateTime thresholdDate)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sp_GetInactiveFeatureUsers";
                var parameter = new { ThresholdDate = thresholdDate };

                var users = await connection.QueryAsync<UserFeatureActivity>(sql, parameter, commandType: CommandType.StoredProcedure);
                return users.ToList();
            }
        }
    }
}
