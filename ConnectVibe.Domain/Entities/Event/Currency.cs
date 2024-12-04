namespace Fliq.Domain.Entities.Event
{
    public class Currency : Record
    {
        public string CurrencyCode { get; set; } = default!;
    }
}