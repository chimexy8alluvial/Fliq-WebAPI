namespace Fliq.Application.DashBoard.Common
{
    public class RefundTicketRequest
    {
        public int EventId { get; set; }
        public int UserId { get; set; }
        public List<int> EventTicketIds { get; set; } = new List<int>();
    }
}
