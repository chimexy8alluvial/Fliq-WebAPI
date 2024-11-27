using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Common.Interfaces.Services.DocumentServices
{
    public interface IMediaServices
    {
        Task<string?> UploadEventMediaAsync(IFormFile docUpload);
    }
}
