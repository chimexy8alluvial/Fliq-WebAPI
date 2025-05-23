namespace Fliq.Application.Recommendations.Common
{
    public record GetUserRecommendationsResult(
        int UserId,
        int RecommendedItemId,
        string ItemType,
        DateTime RecommendationDate,
        double Score
    );
}
