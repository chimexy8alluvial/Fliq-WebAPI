using Fliq.Contracts.Profile;


namespace Fliq.Contracts.Recommendations
{
    public record BlindDateRecommendationDto(
        int Id,
        string Title,
        DateTime StartDateTime,
        LocationDto Location,
        bool IsOneOnOne,
        string? ImageUrl,
        int? NumberOfParticipants,
        int CreatedByUserId,
        double RecommendationScore
    );
}
