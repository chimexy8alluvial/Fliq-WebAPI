namespace Fliq.Domain.Entities.Event
{
    //Class for tickerchases
    public class EventTicket : Record
    {
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public int PaymentId { get; set; }
        public Payment Payment { get; set; } = default!;
    }
}