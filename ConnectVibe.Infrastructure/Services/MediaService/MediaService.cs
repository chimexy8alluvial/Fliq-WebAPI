using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Microsoft.AspNetCore.Http;


namespace Fliq.Infrastructure.Services.MediaService
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
        }
    }
}
