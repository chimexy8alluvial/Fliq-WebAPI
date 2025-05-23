using Fliq.Application.Recommendations.Common;
using Fliq.Contracts.Recommendations;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums.Recommendations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IRecommendationRepository
    {
        //Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedEventsAsync(int userId, int limit);
        //Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedBlindDatesAsync(int userId, int limit);
        //Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedSpeedDatesAsync(int userId, int limit);
        //Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedUsersAsync(int userId, int limit);
        //Task CacheBatchRecommendationsAsync(int userId, IEnumerable<GetUserRecommendationsResult> recommendations);
        //Task<bool> HasCachedRecommendationsAsync(int userId);
        Task<IEnumerable<UserInteraction>> GetPastUserInteractionsAsync(int userId, string eventType);
        Task SaveUserInteractionAsync(UserInteraction interaction);


        // New caching methods
        Task<List<CachedRecommendation>> GetCachedRecommendationsAsync(int userId, RecommendationType type, int count);
        Task SaveCachedRecommendationsAsync(int userId, List<CachedRecommendation> recommendations);
        Task ClearCachedRecommendationsAsync(int userId);
        Task<bool> HasValidCachedRecommendationsAsync(int userId, RecommendationType type, DateTime olderThan);
        Task CleanupOldCachedRecommendationsAsync(DateTime olderThan);

    }
}
