namespace Fliq.Contracts.Games
{
    public class StakeResponseDto
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}