namespace Fliq.Contracts.Polls
{
    public record VoteResponse
    (
        bool SuccessStatus,
        string? Message
    );
}
