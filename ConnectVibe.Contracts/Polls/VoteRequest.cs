namespace Fliq.Contracts.Polls
{
    public record VoteRequest
    (
        int UserId,
        int EventId,
        string Question,
        List<string> Options,
        bool multipleOptionSelect
    );
}
