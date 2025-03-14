

using Microsoft.AspNetCore.Http;

namespace Fliq.Application.DatingEnvironment.Common
{
    public record DatePhotoMapped
    (
     IFormFile BlindDateSessionImageFile
        );
    
}
