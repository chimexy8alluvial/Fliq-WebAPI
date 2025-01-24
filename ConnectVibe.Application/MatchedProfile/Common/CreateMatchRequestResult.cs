namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateMatchRequestResult
    (
        int Id,
        int RequestedUserId,
        int MatchInitiatorUserId,
        string Name,
        string? PictureUrl,
        int? Age,
        int MatchRequestStatus
    );
}
