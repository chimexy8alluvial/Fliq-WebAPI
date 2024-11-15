

namespace Fliq.Domain.Entities.Notifications
{
    public class Notification : Record
    {
        public int UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string? ActionUrl { get; set; }        
        public string? ButtonText { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
