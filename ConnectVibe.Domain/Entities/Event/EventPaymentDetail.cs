namespace Fliq.Domain.Entities.Event
{
    public class EventPaymentDetail : Record
    {
        public string Account { get; set; } = default!;
        public string Bank { get; set; } = default!;
        public string SortCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
    }
}