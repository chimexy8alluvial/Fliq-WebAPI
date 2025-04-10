namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(

       int UserId,
       List<PurchaseTicketDetail> PurchaseTicketDetailList,
        int PaymentId

        );

    public class PurchaseTicketDetail
    {
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public int TicketId { get; set; }
    }
}