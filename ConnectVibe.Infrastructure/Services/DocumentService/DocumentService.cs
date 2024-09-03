using ConnectVibe.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Azure.AI.Vision.Face;
using Azure;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using Microsoft.Extensions.Options;
using ConnectVibe.Infrastructure.Authentication;
using Azure.Storage.Blobs;

namespace Fliq.Infrastructure.Services.DocumentService
{
    public class DocumentService : IDocumentServices
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private readonly string _containerName = "documents";

        public Task<(bool IsVerified, double ConfidenceLevel)> GetDocumentVarificationStat(string sessionId)
        {
            throw new ArgumentNullException();
        }

        public async Task<string?> UploadDocumentAsync(IFormFile docUpload)
        {
            CloudStorageAccount cloudStorageAccount = AzureConnectionString.GetConnectionString();
            if (docUpload == null || docUpload.Length == 0)
            {
                return null;
            }

            var filePath = Path.Combine(_uploadPath, docUpload.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await docUpload.CopyToAsync(stream);
            }

            var fileUrl = filePath;
            return fileUrl;
            //There is option of saving Documents in the Cloud
            // Attempt to create a CloudStorageAccount

            //var containerClient = cloudStorageAccount.CreateCloudFileClient();
            //await containerClient.CreateIfNotExistsAsync();
            //var blobClient = containerClient.GetBlobClient(docUpload.FileName);

            //using (var stream = docUpload.OpenReadStream())
            //{
                //await blobClient.UploadAsync(stream, overwrite: true);
            //}

            //return blobClient;
        }
    }
}
