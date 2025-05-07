
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Microsoft.AspNetCore.Http;

namespace Fliq.Infrastructure.Services
{
    public class DocumentUploadService : IDocumentUploadService
    {
        private readonly IBusinessIdentificationDocumentTypeRepository _documentIdentificationTypeRepository;
        private readonly IMediaServices _mediaServices;
        private readonly ILoggerManager _logger;
        public DocumentUploadService(IBusinessIdentificationDocumentTypeRepository documentIdentificationTypeRepository, IMediaServices mediaServices, ILoggerManager logger)
        {
            _documentIdentificationTypeRepository = documentIdentificationTypeRepository;
            _mediaServices = mediaServices;
            _logger = logger;
        }
        public async Task<DocumentUploadResult> UploadDocumentsAsync(int documentTypeId, IFormFile frontDocument, IFormFile? backDocument)
        {
            _logger.LogInfo($"Uploading documents for document type ID: {documentTypeId}");

            // Validate document type
            var documentType = await _documentIdentificationTypeRepository.GetByIdAsync(documentTypeId);
            if (documentType == null || documentType.IsDeleted)
            {
                _logger.LogWarn($"Document type not found or deleted: ID {documentTypeId}");
                return new DocumentUploadResult { Success = false };
            }

            // Validate front document
            if (frontDocument == null)
            {
                _logger.LogError("Front document is required.");
                return new DocumentUploadResult { Success = false, ErrorMessage = "Front document is required."  };
            }

            // Validate back document based on HasFrontAndBack
            if (documentType.HasFrontAndBack && backDocument == null)
            {
                _logger.LogError("Back document is required for document type with front and back.");
                return new DocumentUploadResult { Success = false, ErrorMessage = "Back document is required for document type with front and back." };
            }
            if (!documentType.HasFrontAndBack && backDocument != null)
            {
                _logger.LogWarn("Back document provided but not required for document type.");
                // Ignore backDocument
            }

            try
            {
                // Define container name for documents
                string containerName = "business-identification-documents";

                // Upload front document with container name
                string? frontDocumentUrl = await _mediaServices.UploadMediaAsync(frontDocument, containerName);
                if (string.IsNullOrEmpty(frontDocumentUrl))
                {
                    _logger.LogError("Failed to upload front document.");
                    return new DocumentUploadResult { Success = false };
                }

                // Upload back document if required
                string? backDocumentUrl = null;
                if (documentType.HasFrontAndBack && backDocument != null)
                {
                    backDocumentUrl = await _mediaServices.UploadMediaAsync(backDocument, containerName);
                    if (string.IsNullOrEmpty(backDocumentUrl))
                    {
                        _logger.LogError("Failed to upload back document.");
                        return new DocumentUploadResult { Success = false, ErrorMessage = "Failed to upload back document." };
                    }
                }

                _logger.LogInfo("Documents uploaded successfully.");
                return new DocumentUploadResult
                {
                    Success = true,
                    FrontDocumentUrl = frontDocumentUrl,
                    BackDocumentUrl = backDocumentUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to upload documents: {ex.Message}");
                return new DocumentUploadResult { Success = false };
            }
        }
    }
}

