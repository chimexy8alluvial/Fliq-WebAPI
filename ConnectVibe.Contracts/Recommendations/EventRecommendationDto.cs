using Fliq.Contracts.Profile;
using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Contracts.Recommendations
{
    // DTOs for recommendations
    public record EventRecommendationDto(
        int Id,
        string EventTitle,
        string EventDescription,
        EventType EventType,
        EventCategory EventCategory,
        DateTime StartDate,
        DateTime EndDate,
        LocationDto Location,
        int MinAge,
        int MaxAge,
        bool SponsoredEvent,
        double RecommendationScore
    );
}
