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
        public StakeStatus StakeStatus { get; set; } = StakeStatus.Pending;
        public bool IsResolved { get; set; } = false;
        public StakeResolutionOption ResolutionOption { get; set; } = StakeResolutionOption.ReturnToPlayers;
    }

    public enum StakeStatus
    {
        Pending = 0,
        Paid = 1,
        Cancelled = 2
    }

    public enum StakeResolutionOption
    {
        WinnerTakesAll = 0,
        ReturnToPlayers = 1,
    }
}