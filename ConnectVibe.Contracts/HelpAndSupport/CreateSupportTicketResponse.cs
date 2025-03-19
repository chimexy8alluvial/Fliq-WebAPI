namespace Fliq.Contracts.HelpAndSupport
{
    public class CreateSupportTicketResponse
    {
        public string SupportTicketId { get; set; } = default!;

        public CreateSupportTicketResponse(string supportTicketId)
        {
            SupportTicketId = supportTicketId;
        }
    }
}