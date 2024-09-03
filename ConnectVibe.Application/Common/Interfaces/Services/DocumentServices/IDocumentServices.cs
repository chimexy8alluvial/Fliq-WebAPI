using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Common.Interfaces.Services.DocumentServices
{
    public interface IDocumentServices
    {
        Task<string?> UploadDocumentAsync(IFormFile docUpload);
        //Task<(bool IsVerified, double ConfidenceLevel)> GetDocumentVarificationStat(string sessionId);
    }
}
