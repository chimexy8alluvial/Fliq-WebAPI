namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateMatchProfileResult
    (
        int MatchInitiatorUserId,
        string Name,
        string PictureUrl
    );
}
