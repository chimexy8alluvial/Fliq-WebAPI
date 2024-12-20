namespace Fliq.Contracts.MatchedProfile
{
    public record MatchRequest(
        int? MatchInitiatorUserId,
        int MatchReceiverUserId
     );
}