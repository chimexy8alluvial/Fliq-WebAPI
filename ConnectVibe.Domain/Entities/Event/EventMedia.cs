using Microsoft.AspNetCore.Http;

namespace Fliq.Domain.Entities.Event
{
    public class EventMedia
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
