
using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.BusinessDocumentType.Command
{
    public record AddBusinessIdentificationDocumentTypeCommand(
       string Name,
       bool HasFrontAndBack
   ) : IRequest<ErrorOr<BusinessIdentificationDocumentTypeResponse>>;

    public class AddBusinessIdentificationDocumentTypeCommandHandler : IRequestHandler<AddBusinessIdentificationDocumentTypeCommand, ErrorOr<BusinessIdentificationDocumentTypeResponse>>
    {
        private readonly IBusinessIdentificationDocumentTypeRepository _documentRepository;
        private readonly ILoggerManager _logger;
        public AddBusinessIdentificationDocumentTypeCommandHandler(IBusinessIdentificationDocumentTypeRepository documentRepository, ILoggerManager logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<BusinessIdentificationDocumentTypeResponse>> Handle(AddBusinessIdentificationDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Adding document type: {command.Name}");
            var existing = _documentRepository.GetAllBusinessIdentificationDocumentTypesAsync();
            if (existing.Any(d => d.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarn($"Document type already exists: {command.Name}");
                return Errors.BusinessDocumentType.DuplicateName;
            }
            var documentType = new Fliq.Domain.Entities.BusinessIdentificationDocumentType
            {
                Name = command.Name,
                HasFrontAndBack = command.HasFrontAndBack
            };
            await _documentRepository.AddBusinessIdentificationDocumentTypeAsync(documentType);
            var result = documentType.Adapt<BusinessIdentificationDocumentTypeResponse>();
            _logger.LogInfo($"Document type added successfully: {result.Name} (ID: {result.Id})");
            return result;
        }
    }
}
