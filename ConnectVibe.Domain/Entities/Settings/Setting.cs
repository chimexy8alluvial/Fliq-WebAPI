using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.Settings
{
    public class Setting : Record
    {
        public ScreenMode ScreenMode { get; set; }
        public bool RelationAvailability { get; set; }
        public bool ShowMusicAndGameStatus { get; set; }
        public Language Language { get; set; } = Language.English;
        public ICollection<NotificationPreference> NotificationPreferences { get; set; } = new List<NotificationPreference>();
        public Filter Filter { get; set; } = new Filter();
        public User User { get; set; } = default!;
        public int UserId { get; set; }
    }

    public enum ScreenMode
    {
        White,
        Dark
    }

    public class HelpCenter : Record
    {
        public int UserId { get; set; }
        public string Message { get; set; } = default!;
        public string FeedBackChannel { get; set; } = default!;
    }
}