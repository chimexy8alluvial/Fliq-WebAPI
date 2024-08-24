using Azure.AI.Vision.Face;
using Azure;
using ConnectVibe.Application.Common.Helpers;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Options;
using ConnectVibe.Infrastructure.Authentication;
using ConnectVibe.Application.Common.Models;

namespace ConnectVibe.Infrastructure.Services.ImageServices
{
    public class ImageService : IImageService
    {
        private readonly FaceApi _faceApi;

        public ImageService(IOptions<FaceApi> faceApiOptions)
        {
            _faceApi = faceApiOptions.Value;
        }

        public async Task<string?> UploadImageAsync(IFormFile imageToUpload)
        {
            if (imageToUpload == null || imageToUpload.Length == 0)
            {
                return null;
            }

            // Validate Image Extension
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(imageToUpload.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(extension))
            {
                return null;
            }

            // Attempt to create a CloudStorageAccount
            CloudStorageAccount cloudStorageAccount = AzureConnectionString.GetConnectionString();

            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("profile-photos");

            string imageName = Guid.NewGuid().ToString() + extension;

            // Validate Image Integrity
            using (var memoryStream = new MemoryStream())
            {
                await imageToUpload.CopyToAsync(memoryStream);
                try
                {
                    memoryStream.Position = 0;
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
            cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

            try
            {
                // Upload the image to Azure Blob Storage
                using var imageStream = imageToUpload.OpenReadStream();
                await cloudBlockBlob.UploadFromStreamAsync(imageStream);
            }
            catch (Exception)
            {
                return null;
            }

            // Return the full path of the uploaded image
            return cloudBlockBlob.Uri.ToString();
        }

        public async Task<string?> UploadMediaAsync(IFormFile mediaToUpload)
        {
            if (mediaToUpload == null || mediaToUpload.Length == 0)
            {
                return null;
            }

            // Validate Media Extension
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", "gif", ".mp4", ".mov", ".avi", ".mkv" };
            var extension = Path.GetExtension(mediaToUpload.FileName).ToLowerInvariant();
            if (!validExtensions.Contains(extension))
            {
                return null;
            }

            // Attempt to create a CloudStorageAccount
            CloudStorageAccount cloudStorageAccount = AzureConnectionString.GetConnectionString();

            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("media");

            string mediaName = Guid.NewGuid().ToString() + extension;

            // Validate Media Integrity
            using (var memoryStream = new MemoryStream())
            {
                await mediaToUpload.CopyToAsync(memoryStream);
                try
                {
                    memoryStream.Position = 0;
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(mediaName);
            cloudBlockBlob.Properties.ContentType = mediaToUpload.ContentType;

            try
            {
                // Upload the media to Azure Blob Storage
                using var mediaStream = mediaToUpload.OpenReadStream();
                await cloudBlockBlob.UploadFromStreamAsync(mediaStream);
            }
            catch (Exception)
            {
                return null;
            }

            // Return the full path of the uploaded media
            return cloudBlockBlob.Uri.ToString();
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
    }
}