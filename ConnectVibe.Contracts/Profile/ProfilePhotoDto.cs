using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Profile
{
    public class ProfilePhotoDto
    {
        public string Caption { get; set; } = default!;

        public IFormFile ImageFile { get; set; } = default!;
    }
}