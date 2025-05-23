using Fliq.Domain.Enums.Recommendations;

namespace Fliq.Contracts.Recommendations
{
    public record TrackUserInteractionRequest(
    int UserId,
    InteractionType InteractionType,
    int? EventId,
    int? BlindDateId,
    int? SpeedDatingEventId,
    int? InteractedWithUserId,
    DateTime InteractionTime,
    double InteractionStrength
    );
}
