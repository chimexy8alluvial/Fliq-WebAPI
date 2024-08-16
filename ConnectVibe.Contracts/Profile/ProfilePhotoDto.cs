using Microsoft.AspNetCore.Http;

namespace ConnectVibe.Contracts.Profile
{
    public record ProfilePhotoDto
    (string Caption, IFormFile ImageFile);
}