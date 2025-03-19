namespace Fliq.Domain.Entities.HelpAndSupport
{
    public class HelpMessage : Record
    {
        public int SupportTicketId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}