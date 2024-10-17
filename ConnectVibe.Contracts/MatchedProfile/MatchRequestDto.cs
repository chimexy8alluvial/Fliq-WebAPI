namespace Fliq.Contracts.MatchedProfile
{
    public class MatchRequestDto
    {
        public int MatchInitiatorUserId { get; set; }
        public string Name { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
    }
}
