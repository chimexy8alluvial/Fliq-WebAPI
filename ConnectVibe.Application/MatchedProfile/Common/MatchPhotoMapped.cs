using Microsoft.AspNetCore.Http;

namespace Fliq.Application.MatchedProfile.Common
{
    public class MatchPhotoMapped
    {
        public string Caption { get; set; } = default!;

        public IFormFile ImageFile { get; set; } = default!;
    }
}
