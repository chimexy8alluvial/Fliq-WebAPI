

using Fliq.Contracts.Dating;

namespace Fliq.Application.DatingEnvironment.Common
{
    public record DeleteDatingEventsResult
    (
         int TotalDeletedCount,
         int BlindDateDeletedCount,
         int SpeedDateDeletedCount
    );
}
