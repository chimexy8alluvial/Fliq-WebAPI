namespace Fliq.Contracts.Event
{
    public record PurchaseTicketDto(
       
       List<PurchaseTicketDetail> PurchaseTicketDetailList,
        int PaymentId
        
        );


    public class PurchaseTicketDetail
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public int TicketId { get; set; }
    }
}