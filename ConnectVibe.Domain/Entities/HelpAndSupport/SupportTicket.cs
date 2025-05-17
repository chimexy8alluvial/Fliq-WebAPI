using Fliq.Domain.Enums;

namespace Fliq.Domain.Entities.HelpAndSupport
{
    public class SupportTicket : Record
    {
        public string TicketId { get; set; } = default!; // Custom ticket ID
        public string Title { get; set; } = default!;
        public int RequesterId { get; set; }
        public string RequesterName { get; set; } = default!;
        public HelpRequestType RequestType { get; set; }
        public HelpRequestStatus RequestStatus { get; set; }
        public int GameSessionId { get; set; }
        public List<HelpMessage> Messages { get; set; } = new List<HelpMessage>();
    }
}