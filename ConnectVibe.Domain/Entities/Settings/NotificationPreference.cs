namespace Fliq.Domain.Entities.Settings
{
    public class NotificationPreference : Record
    {
        public string Context { get; set; } = default!; // e.g., "Messages", "EventReminder"
        public bool PushNotification { get; set; }
        public bool InAppNotification { get; set; }
    }
}