

namespace Fliq.Domain.Entities.Notifications
{
    public class UserDeviceToken : Record
    {
        public int UserId { get; set; }
        public string DeviceToken { get; set; } = default!;

        // Navigation property
        public User User { get; set; } = default!;
    }
}
