using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Games
{
    public class GameRequest : Record
    {
        public int GameId { get; set; }
        public int RequesterId { get; set; }
        public int RecipientId { get; set; }
        public GameStatus Status { get; set; } = GameStatus.Pending;
    }
}