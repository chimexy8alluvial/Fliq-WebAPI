using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Contracts.Dating
{
    public record DeleteDatingEventResponse (
         int TotalDeletedCount,
         int BlindDateDeletedCount,
         int SpeedDateDeletedCount
    );
}
