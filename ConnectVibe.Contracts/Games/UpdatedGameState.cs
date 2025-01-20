namespace Fliq.Contracts.Games
{
    public class UpdatedGameState
    {
        public int SessionId { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int CurrentTurnPlayerId { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public bool IsGameDone { get; set; }
    }
}