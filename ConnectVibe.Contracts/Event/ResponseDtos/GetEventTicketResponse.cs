namespace Fliq.Contracts.Event.ResponseDtos
{
    public class GetEventTicketResponse
    {
        public int TicketId { get; set; }

        public int UserId { get; set; }

        public int PaymentId { get; set; }
    }
}