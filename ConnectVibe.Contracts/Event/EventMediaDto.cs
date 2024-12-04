using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event
{
    public class EventMediaDto
    {
        public string Title { get; set; } = default!;

        public IFormFile DocFile { get; set; } = default!;
    }
}
