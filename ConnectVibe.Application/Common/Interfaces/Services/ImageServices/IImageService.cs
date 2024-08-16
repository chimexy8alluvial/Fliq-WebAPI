using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Common.Interfaces.Services.ImageServices
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageToUpload);
    }
}