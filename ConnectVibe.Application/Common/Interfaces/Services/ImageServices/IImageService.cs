using Azure.AI.Vision.Face;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Common.Interfaces.Services.ImageServices
{
    public interface IImageService
    {
        Task<string?> UploadImageAsync(IFormFile imageToUpload);

        Task<string?> UploadMediaAsync(IFormFile mediaToUpload, string containerName);

        Task<(bool IsVerified, double ConfidenceLevel, LivenessWithVerifySession? LivenessWithVerifySession)> GetFaceLivelinessResult(string sessionId);
    }
}