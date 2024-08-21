using Microsoft.AspNetCore.Http;

namespace ConnectVibe.Application.Common.Interfaces.Services.ImageServices
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile imageToUpload);
    }
}