namespace Fliq.Contracts.Games
{
    public class CreateStakeRequestDto
    {
        public int GameSessionId { get; set; }
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
    }
}