using Fliq.Application.Common.Interfaces.Services.DocumentServices;
using Microsoft.AspNetCore.Http;

namespace Fliq.Infrastructure.Services.DocumentService
{
    public class MediaService : IMediaServices
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private readonly string _containerName = "documents";


        public async Task<string?> UploadEventMediaAsync(IFormFile mediaUpload)
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


            //There is option of saving Documents in the Cloud
            // Validate Media Extension
            //    var validExtensions = new[] { ".jpg", ".jpeg", ".png", "gif", ".mp4", ".mov", ".avi", ".mkv" };
            //    var extension = Path.GetExtension(mediaUpload.FileName).ToLowerInvariant();
            //    if (!validExtensions.Contains(extension))
            //    {
            //        return null;
            //    }

            //    // Attempt to create a CloudStorageAccount
            //    CloudStorageAccount cloudStorageAccount = AzureConnectionString.GetConnectionString();

            //    var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            //    var cloudBlobContainer = cloudBlobClient.GetContainerReference("media");

            //    string mediaName = Guid.NewGuid().ToString() + extension;

            //    // Validate Media Integrity
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        await mediaUpload.CopyToAsync(memoryStream);
            //        try
            //        {
            //            memoryStream.Position = 0;
            //            memoryStream.Seek(0, SeekOrigin.Begin);
            //        }
            //        catch (Exception)
            //        {
            //            return null;
            //        }
            //    }

            //    var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(mediaName);
            //    cloudBlockBlob.Properties.ContentType = mediaUpload.ContentType;

            //    try
            //    {
            //        // Upload the media to Azure Blob Storage
            //        using var mediaStream = mediaUpload.OpenReadStream();
            //        await cloudBlockBlob.UploadFromStreamAsync(mediaStream);
            //    }
            //    catch (Exception)
            //    {
            //        return null;
            //    }

            //    // Return the full path of the uploaded media
        }   //    return cloudBlockBlob.Uri.ToString();
            //}
    }
}
