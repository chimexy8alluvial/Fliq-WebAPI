using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Profile.Common
{
    public class ProfilePhotoMapped
    {
        public string Caption { get; set; } = default!;

        public IFormFile ImageFile { get; set; } = default!;
    }
}
