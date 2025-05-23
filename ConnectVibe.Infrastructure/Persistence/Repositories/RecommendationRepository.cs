using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Common;
using Fliq.Contracts.Recommendations;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums.Recommendations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Data;
using System.Threading;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerManager _logger;

        public RecommendationRepository(IDbConnectionFactory connectionFactory, ILoggerManager logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
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

        public async Task<IEnumerable<UserInteraction>> GetPastUserInteractionsAsync(int userId, string eventType)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@EventType", eventType);

            _logger.LogInfo($"Getting past {eventType} interactions for user {userId}");

            var interactions = await connection.QueryAsync<UserInteraction>(
                "sPGetPastUserInteractions",
                parameters,
                commandType: CommandType.StoredProcedure);

            _logger.LogInfo($"Found {interactions.Count()} past {eventType} interactions for user {userId}");
            return interactions;
        }

        public async Task<List<CachedRecommendation>> GetCachedRecommendationsAsync(int userId, RecommendationType type, int count)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@RecommendationType", type.ToString());
            parameters.Add("@Count", count);

            _logger.LogInfo($"Getting cached {type} recommendations for user {userId}");

            var recommendations = await connection.QueryAsync<CachedRecommendation>(
                "sPGetCachedRecommendations",
                parameters,
                commandType: CommandType.StoredProcedure);

            var result = recommendations.ToList();
            _logger.LogInfo($"Found {result.Count} cached {type} recommendations for user {userId}");
            return result;
        }

        public async Task SaveCachedRecommendationsAsync(int userId, List<CachedRecommendation> recommendations)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                _logger.LogInfo($"Saving {recommendations.Count} cached recommendations for user {userId}");

                // Clear existing recommendations first
                var clearParameters = new DynamicParameters();
                clearParameters.Add("@UserId", userId);

                await connection.ExecuteAsync(
                    "sPClearCachedRecommendations",
                    clearParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                // Bulk insert new recommendations
                var insertParameters = new DynamicParameters();
                insertParameters.Add("@UserId", userId);
                insertParameters.Add("@RecommendationsJson", JsonConvert.SerializeObject(recommendations));

                await connection.ExecuteAsync(
                    "sPSaveCachedRecommendations",
                    insertParameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                transaction.Commit();
                _logger.LogInfo($"Successfully saved cached recommendations for user {userId}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"Failed to save cached recommendations for user {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task ClearCachedRecommendationsAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@DeletedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _logger.LogInfo($"Clearing cached recommendations for user {userId}");

            await connection.ExecuteAsync(
                "sPClearCachedRecommendations",
                parameters,
                commandType: CommandType.StoredProcedure);

            var deletedCount = parameters.Get<int>("@DeletedCount");
            _logger.LogInfo($"Cleared {deletedCount} cached recommendations for user {userId}");
        }

        public async Task<bool> HasValidCachedRecommendationsAsync(int userId, RecommendationType type, DateTime olderThan)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@RecommendationType", type.ToString());
            parameters.Add("@OlderThan", olderThan);
            parameters.Add("@HasValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "sPHasValidCachedRecommendations",
                parameters,
                commandType: CommandType.StoredProcedure);

            return parameters.Get<bool>("@HasValid");
        }

        public async Task CleanupOldCachedRecommendationsAsync(DateTime olderThan)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OlderThan", olderThan);
            parameters.Add("@DeletedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            _logger.LogInfo($"Cleaning up cached recommendations older than {olderThan}");

            await connection.ExecuteAsync(
                "sPCleanupOldCachedRecommendations",
                parameters,
                commandType: CommandType.StoredProcedure);

            var deletedCount = parameters.Get<int>("@DeletedCount");
            _logger.LogInfo($"Cleaned up {deletedCount} old cached recommendations");
        }

    }
}
