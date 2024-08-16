using ConnectVibe.Application.Common.Helpers;
using ConnectVibe.Application.Common.Interfaces.Services.ImageServices;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ConnectVibe.Infrastructure.Services.ImageServices
{
    public class ImageService : IImageService
    {
        public async Task<string> UploadImageAsync(IFormFile imageToUpload)
        {
            string imageFullPath = null;
            if (imageToUpload == null || imageToUpload.Length == 0)
            {
                return null;
            }

            CloudStorageAccount cloudStorageAccount = AzureConnectionString.GetConnectionString();
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("profile-photos");

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                    );
            }
            string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
            cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;
            await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.OpenReadStream());

            imageFullPath = cloudBlockBlob.Uri.ToString();
            return imageFullPath;
        }
    }
}