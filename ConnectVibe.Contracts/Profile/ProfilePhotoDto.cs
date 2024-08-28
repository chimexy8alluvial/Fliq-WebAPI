using Microsoft.AspNetCore.Http;

namespace ConnectVibe.Contracts.Profile
{
    public class ProfilePhotoDto
    {
        public string Caption { get; set; } = default!;

        public IFormFile ImageFile { get; set; } = default!;
    }
}