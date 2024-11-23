using Microsoft.AspNetCore.Http;


namespace Fliq.Application.Common.Interfaces.Services.MeidaServices
{
    public interface IMediaServices
    {
        Task<string?> UploadEventMediaAsync(IFormFile docUpload);
    }
}
