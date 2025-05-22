using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Common;
using Fliq.Contracts.Recommendations;
using Fliq.Domain.Entities.Recommendations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Threading;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerManager _logger;
        private FliqDbContext _dbContext;

        public RecommendationRepository(IDbConnectionFactory connectionFactory, ILoggerManager logger, FliqDbContext dbContext)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedEventsAsync(int userId, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPGetRecommendedEvents";
                var parameters = new { UserId = userId, Limit = limit };

                _logger.LogInfo($"Fetching up to {limit} recommended events for user {userId}");
                var recommendations = await connection.QueryAsync<GetUserRecommendationsResult>(
                    sql, parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInfo($"Retrieved {recommendations.Count()} event recommendations for user {userId}");
                return recommendations;
            }
        }

        public async Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedBlindDatesAsync(int userId, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPGetRecommendedBlindDates";
                var parameters = new { UserId = userId, Limit = limit };

                _logger.LogInfo($"Fetching up to {limit} recommended blind dates for user {userId}");
                var recommendations = await connection.QueryAsync<GetUserRecommendationsResult>(
                    sql, parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInfo($"Retrieved {recommendations.Count()} blind date recommendations for user {userId}");
                return recommendations;
            }
        }

        public async Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedSpeedDatesAsync(int userId, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPGetRecommendedSpeedDates";
                var parameters = new { UserId = userId, Limit = limit };

                _logger.LogInfo($"Fetching up to {limit} recommended speed dates for user {userId}");
                var recommendations = await connection.QueryAsync<GetUserRecommendationsResult>(
                    sql, parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInfo($"Retrieved {recommendations.Count()} speed date recommendations for user {userId}");
                return recommendations;
            }
        }

        public async Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedUsersAsync(int userId, int limit)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPGetRecommendedUsers";
                var parameters = new { UserId = userId, Limit = limit };

                _logger.LogInfo($"Fetching up to {limit} recommended users for user {userId}");
                var recommendations = await connection.QueryAsync<GetUserRecommendationsResult>(
                    sql, parameters, commandType: CommandType.StoredProcedure);

                _logger.LogInfo($"Retrieved {recommendations.Count()} user recommendations for user {userId}");
                return recommendations;
            }
        }

        public async Task SaveUserInteractionAsync(UserInteraction interaction)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPSaveUserInteraction";
                var parameters = new DynamicParameters();

                parameters.Add("@UserId", interaction.UserId);
                parameters.Add("@InteractionType", interaction.Type);
                parameters.Add("@EventId", interaction.EventId);
                parameters.Add("@BlindDateId", interaction.BlindDateId);
                parameters.Add("@SpeedDatingEventId", interaction.SpeedDatingEventId);
                parameters.Add("@InteractedWithUserId", interaction.InteractedWithUserId);
                parameters.Add("@InteractionTime", interaction.InteractionTime);
                parameters.Add("@InteractionStrength", interaction.InteractionStrength);

                _logger.LogInfo($"Saving user interaction for user {interaction.UserId}, type: {interaction.Type}");
                await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                _logger.LogInfo($"Successfully saved user interaction");
            }
        }

        public async Task CacheBatchRecommendationsAsync(int userId, IEnumerable<GetUserRecommendationsResult> recommendations)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                // Using a table-valued parameter would be ideal here, but for simplicity:
                foreach (var recommendation in recommendations)
                {
                    var sql = "sPCacheRecommendation";
                    var parameters = new DynamicParameters();

                    parameters.Add("@UserId", userId);
                    parameters.Add("@RecommendedItemId", recommendation.RecommendedItemId);
                    parameters.Add("@ItemType", recommendation.ItemType);
                    parameters.Add("@Score", recommendation.Score);

                    await connection.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);
                }

                _logger.LogInfo($"Cached {recommendations.Count()} recommendations for user {userId}");
            }
        }

        public async Task<bool> HasCachedRecommendationsAsync(int userId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "sPCheckCachedRecommendations";
                var parameters = new { UserId = userId };

                var count = await connection.ExecuteScalarAsync<int>(sql, parameters, commandType: CommandType.StoredProcedure);
                return count > 0;
            }
        }

        public async Task<IEnumerable<UserInteraction>> GetPastUserInteractionsAsync(int userId, string eventType)
        {
             List<UserInteraction> result = new();
            switch (eventType.ToLower())
            {
                case "event":
                    result = await _dbContext.UserInteractions
                .Where(ui => ui.UserId == userId && ui.EventId != null)
                .Include(ui => ui.Event)
                .ToListAsync();
                    break;
                case "blinddate":

                    result = await _dbContext.UserInteractions
               .Where(ui => ui.UserId == userId && ui.BlindDateId != null)
               .Include(ui => ui.BlindDate)
               .ToListAsync();
                    break;

                case "speeddate":

                    result = await _dbContext.UserInteractions
               .Where(ui => ui.UserId == userId && ui.SpeedDatingEventId != null)
               .Include(ui => ui.SpeedDatingEvent)
               .ToListAsync();
                    break;
            }
            return result;
        }
    }
}
