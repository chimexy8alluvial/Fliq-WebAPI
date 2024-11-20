namespace Fliq.Contracts.Polls
{
    public record PollRequest
    (
        int UserId,
        int EventId,
        string Question,
        List<string> Options,
        bool multipleOptionSelect
    );
}
