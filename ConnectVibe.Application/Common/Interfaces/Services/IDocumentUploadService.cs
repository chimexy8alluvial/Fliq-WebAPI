using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Common.Interfaces.Services
{
    public interface IDocumentUploadService
    {
        Task<DocumentUploadResult> UploadDocumentsAsync(int documentTypeId, IFormFile frontDocument, IFormFile? backDocument);
    }

    public record DocumentUploadResult
    {
        public bool Success { get; init; }
        public string? FrontDocumentUrl { get; init; }
        public string? BackDocumentUrl { get; init; }
    }
}

