namespace Fliq.Contracts.MatchedProfile
{
    public class MatchRequestDto
    {
        public int MatchInitiatorUserId { get; set; }
        public int MatchReceiverUserId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public int? Age { get; set; }
    }
}