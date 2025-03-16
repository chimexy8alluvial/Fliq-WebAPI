

using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Dating
{
    public record BlindDatePhotoDto 
    (
       IFormFile BlindDateSessionImageFile
    );
}
