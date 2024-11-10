

namespace Fliq.Domain.Entities.Notifications
{
    public class Notification : Record
    {
        public int UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public bool IsRead { get; set; } = false;
    }
}
