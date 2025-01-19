namespace Fliq.Contracts.Games
{
    public class GameSessionResponse
    {
        public int GameId { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? WinnerId { get; set; }
        public int StakeId { get; set; }
        public decimal StakeAmount { get; set; }
    }
}