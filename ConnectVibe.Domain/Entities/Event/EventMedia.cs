namespace Fliq.Domain.Entities.Event
{
    public class EventMedia : Record
    {
        public string MediaUrl { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}