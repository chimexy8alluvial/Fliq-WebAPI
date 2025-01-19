namespace Fliq.Domain.Entities.Games
{
    public class Stake : Record
    {
        public int GameSessionId { get; set; }
        public GameSession? GameSession { get; set; }
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public decimal Amount { get; set; }
        public bool IsAccepted { get; set; }
    }
}