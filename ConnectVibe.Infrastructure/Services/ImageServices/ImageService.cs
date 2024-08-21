using ConnectVibe.Application.Common.Helpers;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;

namespace ConnectVibe.Infrastructure.Services.ImageServices
{
    public class ImageService : IImageService
    {
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
    }
}