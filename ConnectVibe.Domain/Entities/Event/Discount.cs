using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class Discount : Record
    {
        public string? Name { get; set; }
        public DiscountType Type { get; set; }
        public double Percentage { get; set; }

        // Specific to Number of Tickets discount type
        public int? NumberOfTickets { get; set; }
    }
}