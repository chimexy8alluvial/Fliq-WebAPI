namespace Fliq.Contracts.Event.UpdateDtos
{
    public class UpdateTicketDto
    {
        public int Id { get; set; }
        public string? TicketName { get; set; }
        public int? TicketType { get; set; }
        public string? TicketDescription { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Currency { get; set; }
        public decimal? Amount { get; set; }
        public string? MaximumLimit { get; set; }
        public bool? SoldOut { get; set; }
        public List<UpdateDiscountDto>? Discounts { get; set; }
        public int EventId { get; set; }
    }
}