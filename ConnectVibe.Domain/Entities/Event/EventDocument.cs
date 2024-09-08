using Microsoft.AspNetCore.Http;

namespace Fliq.Domain.Entities.Event
{
    public class EventDocument
    {
        public int Id { get; set; }
        public string DocumentUrl { get; set; } = default!;
        public string Title { get; set; } = default!;
        public IFormFile Documentfile { get; set; } = default!;
    }
    public class EvtDocumentDto
    {
        public string Title { get; set; } = default!;
        public IFormFile Documentfile { get; set; } = default!;
    }
}
