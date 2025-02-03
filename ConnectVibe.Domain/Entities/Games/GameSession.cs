using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Games
{
    public class GameSession : Record
    {
        public int GameId { get; set; }
        public Game Game { get; set; } = default!;
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int Player1Score { get; set; } = 0;
        public int Player2Score { get; set; } = 0;
        public GameStatus Status { get; set; } = GameStatus.Pending;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Stake? Stake { get; set; }
    }
}