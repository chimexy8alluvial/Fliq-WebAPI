namespace Fliq.Domain.Entities.Games
{
    public class GameQuestion : Record
    {
        public Game Game { get; set; } = default!;
        public int GameId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string>? Options { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Level { get; set; }
        public string? Theme { get; set; }
    }
}