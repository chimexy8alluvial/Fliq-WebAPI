using Azure.AI.Vision.Face;
using Microsoft.AspNetCore.Http;


namespace Fliq.Application.Common.Interfaces.Services.MeidaServices
{
    public interface IMediaServices
    {
        Task<string?> UploadImageAsync(IFormFile imageToUpload);

        Task<string?> UploadMediaAsync(IFormFile mediaToUpload, string containerName);
        Task<string?> UploadDocumentAsync(byte[] fileBytes, string fileName, string containerName);

        Task<(bool IsVerified, double ConfidenceLevel, LivenessWithVerifySession? LivenessWithVerifySession)> GetFaceLivelinessResult(string sessionId);
    }
}
