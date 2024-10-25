using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class Discount : Record
    {
        public string Name { get; set; }
        public DiscountType Type { get; set; }
        public double Percentage { get; set; }

        // Specific to Number of Tickets discount type
        public int? NumberOfTickets { get; set; }

        // Constructor for Number of Tickets Discount
        public Discount(string name, double percentage, int numberOfTickets)
        {
            Name = name;
            Type = DiscountType.NumberOfTickets;
            Percentage = percentage;
            NumberOfTickets = numberOfTickets;
        }

        // Constructor for Voucher Discount
        public Discount(string name, double percentage)
        {
            Name = name;
            Type = DiscountType.Voucher;
            Percentage = percentage;
            NumberOfTickets = null; // No tickets for voucher type
        }
    }
}