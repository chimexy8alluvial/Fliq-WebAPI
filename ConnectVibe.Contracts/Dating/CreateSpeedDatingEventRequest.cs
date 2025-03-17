

using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Dating
{
    public record CreateSpeedDatingEventRequest(
        string Title,
        int SpeedDatingCategory,
        DateTime StartTime,
        int MinAge,
        int MaxAge,
        int MaxParticipants,
        int DurationPerPairingMinutes,
       DatePhotoDto? SpeedDatingImage,
       LocationDto Location
        );
}
