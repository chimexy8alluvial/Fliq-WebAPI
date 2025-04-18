
using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.BusinessDocumentType.Command
{
    public record AddBusinessDocumentTypeCommand(
       string Name,
       bool HasFrontAndBack
   ) : IRequest<ErrorOr<BusinessDocumentTypeResponse>>;

    public class AddBusinessDocumentTypeCommandHandler : IRequestHandler<AddBusinessDocumentTypeCommand, ErrorOr<BusinessDocumentTypeResponse>>
    {
        private readonly IBusinessDocumentTypeRepository _documentRepository;
        private readonly ILoggerManager _logger;
        public AddBusinessDocumentTypeCommandHandler(IBusinessDocumentTypeRepository documentRepository, ILoggerManager logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<BusinessDocumentTypeResponse>> Handle(AddBusinessDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Adding document type: {command.Name}");
            var existing =  _documentRepository.GetAllBusinessDocumentTypesAsync();
            if (existing.Any(d => d.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarn($"Document type already exists: {command.Name}");
                return Errors.BusinessDocumentType.DuplicateName;
            }
            var documentType = new Fliq.Domain.Entities.BusinessDocumentType
            {
                Name = command.Name,
                HasFrontAndBack = command.HasFrontAndBack
            };
            await _documentRepository.AddBusinessDocumentTypeAsync(documentType);
            var result = documentType.Adapt<BusinessDocumentTypeResponse>();
            _logger.LogInfo($"Document type added successfully: {result.Name} (ID: {result.Id})");
            return result;
        }
    }
}
