using Fliq.Contracts.Profile;
using Fliq.Domain.Enums;

namespace Fliq.Contracts.Recommendations
{
    public record SpeedDatingRecommendationDto(
        int Id,
        string Title,
        SpeedDatingCategory Category,
        DateTime StartTime,
        string? ImageUrl,
        int MinAge,
        int MaxAge,
        int MaxParticipants,
        LocationDto Location,
        double RecommendationScore
    );
}
