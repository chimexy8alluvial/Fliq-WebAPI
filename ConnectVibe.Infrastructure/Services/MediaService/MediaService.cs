using Azure;
using Azure.AI.Vision.Face;
using Azure.Storage.Blobs;
using Fliq.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Infrastructure.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Diagnostics;


namespace Fliq.Infrastructure.Services.MediaService
{
    public class MediaService : IMediaServices
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private readonly FaceApi _faceApi;
        private readonly ILoggerManager _logger;
        public MediaService(IOptions<FaceApi> faceApiOptions, ILoggerManager logger)
        {
            _faceApi = faceApiOptions.Value;
            _logger = logger;
        }

        public async Task<string?> UploadImageAsync(IFormFile imageToUpload)
        {
           return await UploadMediaAsync(imageToUpload, _uploadPath);
        }

        public async Task<string?> UploadMediaAsync(IFormFile mediaToUpload, string containerName)
        {
            if (mediaToUpload == null || mediaToUpload.Length == 0)
            {
                return null;
            }
            // Check if the app is in Debug Mode
            if (Debugger.IsAttached)
            {
                // In Debug mode, save the file to a local directory instead of uploading to the server
                return await UploadMediaToLocal(mediaToUpload);
            }
            else
            {
                // Validate Media Extension
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", "gif", ".mp4", ".mov", ".avi", ".mkv", ".mp3", ".wav" };
                var extension = Path.GetExtension(mediaToUpload.FileName).ToLowerInvariant();
                if (!validExtensions.Contains(extension))
                {
                    return null;
                }
                // Attempt to create a CloudStorageAccount
                try
                {
                    // Create BlobServiceClient using the connection string
                    BlobServiceClient blobServiceClient = AzureConnectionString.GetConnectionString();

                    // Get or create the BlobContainerClient
                    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    await blobContainerClient.CreateIfNotExistsAsync();

                    // Generate a unique media name
                    string mediaName = Guid.NewGuid().ToString() + extension;

                    // Get a BlobClient for the media
                    BlobClient blobClient = blobContainerClient.GetBlobClient(mediaName);

                    // Upload the media to Azure Blob Storage
                    using (var mediaStream = mediaToUpload.OpenReadStream())
                    {
                        await blobClient.UploadAsync(mediaStream, true);
                    }

                    // Return the full URL of the uploaded media
                    return blobClient.Uri.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred: {ex.Message}");
                    return null;
                }
            }

        }

        public async Task<string> StartFaceLivelinessSession()
        {
            var endpoint = new Uri(_faceApi.Endpoint);
            var credential = new AzureKeyCredential(_faceApi.ApiKey);

            var sessionClient = new FaceSessionClient(endpoint, credential);

            var createContent = new CreateLivenessSessionContent(LivenessOperationMode.Passive)
            {
                DeviceCorrelationId = "723d6d03-ef33-40a8-9682-23a1feb7bccd"
            };
            using var fileStream = new FileStream("test.png", FileMode.Open, FileAccess.Read);

            var createResponse = await sessionClient.CreateLivenessWithVerifySessionAsync(createContent, fileStream);
            var sessionId = createResponse.Value.SessionId;

            return sessionId;
        }

        public async Task<(bool IsVerified, double ConfidenceLevel, LivenessWithVerifySession? LivenessWithVerifySession)> GetFaceLivelinessResult(string sessionId)
        {
            var endpoint = new Uri(_faceApi.Endpoint);
            var credential = new AzureKeyCredential(_faceApi.ApiKey);

            var sessionClient = new FaceSessionClient(endpoint, credential);
            var getResultResponse = await sessionClient.GetLivenessWithVerifySessionResultAsync(sessionId);
            var sessionResult = getResultResponse.Value;

            // Check if the liveness decision was "realface" and the verification was successful
            var isVerified = sessionResult?.Result?.Response?.Body?.VerifyResult?.IsIdentical == true
                             && sessionResult?.Result?.Response?.Body?.LivenessDecision == "realface";

            // Get the match confidence
            var confidenceLevel = sessionResult?.Result?.Response?.Body?.VerifyResult?.MatchConfidence ?? 0.0;

            return (isVerified, confidenceLevel, sessionResult);
        }

        private async Task<string?> UploadMediaToLocal(IFormFile mediaUpload)
        {
            // Saving the Media to a Local Directory
            if (mediaUpload == null || mediaUpload.Length == 0)
            {
                return null;
            }

            var filePath = Path.Combine(_uploadPath, mediaUpload.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await mediaUpload.CopyToAsync(stream);
            }

            var fileUrl = filePath;
            return fileUrl;
        }

    }
}
