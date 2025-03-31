

using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Dating
{
    public record DatePhotoDto 
    (
       IFormFile DateSessionImageFile
    );
}
