namespace Fliq.Domain.Entities.Settings
{
    public class Setting
    {
        public int Id { get; set; }
        public ScreenMode ScreenMode { get; set; }
        public bool RelationAvailability { get; set; }
        public bool ShowMusicAndGameStatus { get; set; }
        public string Language { get; set; } = "English";
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

    public class HelpCenter
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = default!;
        public string FeedBackChannel { get; set; } = default!;
    }
}