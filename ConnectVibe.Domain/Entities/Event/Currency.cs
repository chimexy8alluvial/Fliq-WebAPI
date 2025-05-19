namespace Fliq.Domain.Entities.Event
{
    public class Currency : Record
    {
        public string CurrencyCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string CountryCode { get; set; } = default!;
    }
}