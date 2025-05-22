namespace Fliq.Contracts.Recommendations
{
    public record UserInteractionDto(
    int UserId,
    string InteractionType,
    int? EventId,
    int? BlindDateId,
    int? SpeedDatingEventId,
    int? InteractedWithUserId,
    DateTime InteractionTime,
    double InteractionStrength
    );
}
