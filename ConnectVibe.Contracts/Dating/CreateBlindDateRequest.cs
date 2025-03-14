using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Dating
{
    public record CreateBlindDateRequest(
        int CategoryId,
       string Title,
       DateTime StartDateTime,
       bool IsOneOnOne,
       int? NumberOfParticipants,
       bool IsRecordingEnabled,
       DateTime? SessionStartTime,
       DateTime? SessionEndTime,
       BlindDatePhotoDto? BlindDateImage,
       LocationDto Location
        );
}
