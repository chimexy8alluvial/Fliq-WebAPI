using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event.UpdateDtos
{
    public class UpdateEventMediaDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }

        public IFormFile? DocFile { get; set; }
    }
}