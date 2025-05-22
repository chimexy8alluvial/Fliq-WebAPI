using Fliq.Application.Recommendations.Common;
using Fliq.Contracts.Recommendations;
using Fliq.Domain.Entities.Recommendations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IRecommendationRepository
    {
        Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedEventsAsync(int userId, int limit);
        Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedBlindDatesAsync(int userId, int limit);
        Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedSpeedDatesAsync(int userId, int limit);
        Task<IEnumerable<GetUserRecommendationsResult>> GetRecommendedUsersAsync(int userId, int limit);
        Task<IEnumerable<UserInteraction>> GetPastUserInteractionsAsync(int userId, string eventType);
        Task SaveUserInteractionAsync(UserInteraction interaction);
        Task CacheBatchRecommendationsAsync(int userId, IEnumerable<GetUserRecommendationsResult> recommendations);
        Task<bool> HasCachedRecommendationsAsync(int userId);
    }
}
