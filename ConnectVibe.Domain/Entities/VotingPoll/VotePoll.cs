namespace Fliq.Domain.Entities.VotingPoll
{
    public class VotePoll : Record
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int Count { get; set; }
        public string Question { get; set; } = default!;
        public List<string> Options { get; set; } = default!;
        public bool multipleOptionSelect { get; set; }

    }
}
