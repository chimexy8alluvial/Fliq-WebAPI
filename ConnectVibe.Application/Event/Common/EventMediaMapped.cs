
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Event.Common
{
    public class EventMediaMapped
    {
        public string Title { get; set; } = default!;

        public IFormFile DocFile { get; set; } = default!;
    }
}
