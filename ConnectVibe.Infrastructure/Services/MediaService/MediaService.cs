using Azure;
using Azure.AI.Vision.Face;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        private readonly string _uploadPath;
        private readonly FaceApi _faceApi;
        private readonly ILoggerManager _logger;
        private readonly string _documentPath;

        public MediaService(IOptions<FaceApi> faceApiOptions, ILoggerManager logger)
        {
            _faceApi = faceApiOptions.Value;
            _logger = logger;
            // Get the base directory of the application and combine it with "Uploads"
            _uploadPath = Path.Combine(AppContext.BaseDirectory, "Uploads");
            _documentPath = Path.Combine(_uploadPath, "Documents");

            // Ensure the directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string?> UploadImageAsync(IFormFile imageToUpload)
        {
            return await UploadMediaAsync(imageToUpload, _uploadPath);
        }

        public async Task<string?> UploadMediaAsync(IFormFile mediaToUpload, string containerName)
        {
            if (mediaToUpload == null || mediaToUpload.Length == 0)
            {
                _logger.LogWarn("Document upload failed: File is null or empty.");
                return null;
            }

            // Check if the document is a PDF or other accepted document format
            var validDocumentExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".avi", ".mkv", ".mp3", ".wav", ".pdf", ".doc" };
            var extension = Path.GetExtension(mediaToUpload.FileName).ToLowerInvariant();

            if (!validDocumentExtensions.Contains(extension))
            {
                _logger.LogWarn($"Document upload failed: Invalid file extension '{extension}'");
                return null;
            }

            // Use same container name for all documents or use a specific one for documents
            containerName = "documents";

            // Check if the app is in Debug Mode
            if (Debugger.IsAttached)
            {
                // In Debug mode, save the file to a local directory
                return await UploadDocToLocal(mediaToUpload);
            }
            else
            {
                try
                {
                    // Create BlobServiceClient using the connection string
                    BlobServiceClient blobServiceClient = AzureConnectionString.GetConnectionString();

                    // Get or create the BlobContainerClient
                    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                    await blobContainerClient.CreateIfNotExistsAsync();

                    // Generate a unique document name
                    string documentName = Guid.NewGuid().ToString() + extension;

                    // Get a BlobClient for the document
                    BlobClient blobClient = blobContainerClient.GetBlobClient(documentName);

                    // Set content type based on extension
                    BlobUploadOptions options = new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = GetContentType(extension)
                        }
                    };

                    // Upload the document to Azure Blob Storage
                    using (var documentStream = mediaToUpload.OpenReadStream())
                    {
                        await blobClient.UploadAsync(documentStream, options);
                    }

                    _logger.LogInfo($"Document uploaded successfully: {documentName}");
                    // Return the full URL of the uploaded document
                    return blobClient.Uri.ToString();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Document upload failed: {ex.Message}");
                    return null;
                }
            }
        }

        private string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                ".avi" => "video/x-msvideo",
                ".mkv" => "video/x-matroska",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };
        }

        public async Task<string?> UploadDocumentAsync(byte[] fileBytes, string fileName, string containerName)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return null;
            }

            //// Check if the app is in Debug Mode
            //if (Debugger.IsAttached)
            //{
            //    // In Debug mode, save the file to a local directory instead of uploading to the server
            //    return await UploadDocToLocal(documentToUpload);
            //}


            try
            {
                BlobServiceClient blobServiceClient = AzureConnectionString.GetConnectionString();
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await blobContainerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

                using (var stream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(stream, true);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Document upload failed: {ex.Message}");
                return null;
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

        //Local storage methods
        private async Task<string?> UploadMediaToLocal(IFormFile mediaUpload)
        {
            /// Validate document
            if (mediaUpload == null || mediaUpload.Length == 0)
            {
                _logger.LogWarn("Local document upload failed: File is null or empty.");
                return null;
            }

            try
            {
                // Create a unique filename to prevent overwrites
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(mediaUpload.FileName)}";
                var filePath = Path.Combine(_documentPath, uniqueFileName);

                // Ensure directory exists
                Directory.CreateDirectory(_documentPath);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaUpload.CopyToAsync(stream);
                }

                _logger.LogInfo($"Document saved locally: {uniqueFileName}");
                // For local storage, you might want to use a relative path or URL that your app can access
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Local document upload failed: {ex.Message}");
                return null;
            }
        }

        private async Task<string?> UploadDocToLocal(IFormFile docUpload)
        {
            // Saving the Media to a Local Directory
            if (docUpload == null || docUpload.Length == 0)
            {
                return null;
            }

            var filePath = Path.Combine(_documentPath, docUpload.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await docUpload.CopyToAsync(stream);
            }

            var fileUrl = filePath;
            return fileUrl;
        }
    }
}