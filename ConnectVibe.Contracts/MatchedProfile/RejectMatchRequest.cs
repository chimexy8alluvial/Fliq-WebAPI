namespace Fliq.Contracts.MatchedProfile
{
    public record RejectMatchRequest
    (
        int? Id,
        int UserId
    );
}
