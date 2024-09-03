using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event
{
    public class EventDocumentDto
    {
        public string Tilte { get; set; } = default!;

        public IFormFile Documentfile { get; set; } = default!;
    }
}
