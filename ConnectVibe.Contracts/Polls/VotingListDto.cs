namespace Fliq.Contracts.Polls
{
    public class VotingListDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int Count { get; set; }
        public string Question { get; set; } = default!;
        public List<string> Options { get; set; } = default!;
        public bool multipleOptionSelect { get; set; }
        public List<string> Picture { get; set; } = default!;
    }
}
