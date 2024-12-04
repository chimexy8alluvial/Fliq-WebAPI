namespace Fliq.Contracts.Event.UpdateDtos
{
    public class UpdateDiscountDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Type { get; set; }
        public double? Percentage { get; set; }

        // Specific to Number of Tickets discount type
        public int? NumberOfTickets { get; set; }
    }
}