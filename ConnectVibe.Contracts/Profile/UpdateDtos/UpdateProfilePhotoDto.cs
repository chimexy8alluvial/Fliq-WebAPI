using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Profile.UpdateDtos
{
    public class UpdateProfilePhotoDto
    {
        public int Id { get; set; }
        public string? Caption { get; set; } = default!;

        public IFormFile? ImageFile { get; set; } = default!;
    }
}