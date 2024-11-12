namespace Fliq.Contracts.MatchedProfile
{
    public record MatchRequest(
        int UserId,
        int? MatchInitiatorUserId
     );
}
