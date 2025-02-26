

using Microsoft.AspNetCore.Http;

namespace Fliq.Application.DatingEnvironment.Common
{
    public class BlindDatePhotoMapped
    {
        public IFormFile BlindDateSessionImageFile { get; set; } = default!;
    }
}
