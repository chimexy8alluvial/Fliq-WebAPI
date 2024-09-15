using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event
{
    public class EventMediaDto
    {
        public string Title { get; set; } = default!;

        public IFormFile Documentfile { get; set; } = default!;
    }
}
