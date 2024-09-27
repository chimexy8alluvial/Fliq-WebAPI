namespace Fliq.Domain.Entities.Settings
{
    public class NotificationPreference
    {
        public int Id { get; set; }
        public string Context { get; set; } = default!; // e.g., "Messages", "EventReminder"
        public bool PushNotification { get; set; }
        public bool InAppNotification { get; set; }
    }
}