namespace Fliq.Contracts.Event
{
    public class DiscountDto
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public double Percentage { get; set; }

        // Specific to Number of Tickets discount type
        public int? NumberOfTickets { get; set; }
    }
}