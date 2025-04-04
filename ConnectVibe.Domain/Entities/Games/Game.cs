using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Games
{
    public class Game : Record
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool RequiresLevel { get; set; }
        public bool RequiresTheme { get; set; }
        public bool RequiresCategory { get; set; }
        public GameCreationStatus CreationStatus { get; set; }
    }
}