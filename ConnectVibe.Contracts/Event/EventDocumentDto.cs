using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event
{
    public class EventDocumentDto
    {
        public string Title { get; set; } = default!;

        public IFormFile Documentfile { get; set; } = default!;
        //public string Documentfile { get; set; } = default!;
    }
}
