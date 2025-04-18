using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.BusinessDocumentType.Command
{
    public record DeleteBusinessDocumentTypeCommand(int Id) : IRequest<ErrorOr<Deleted>>;

    public class DeleteBusinessDocumentTypeCommandHandler : IRequestHandler<DeleteBusinessDocumentTypeCommand, ErrorOr<Deleted>>
    {
        private readonly IBusinessDocumentTypeRepository _documentTypeRepository;
        private readonly ILoggerManager _logger;

        public DeleteBusinessDocumentTypeCommandHandler(
            IBusinessDocumentTypeRepository documentTypeRepository,
            ILoggerManager logger)
        {
            _documentTypeRepository = documentTypeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Deleted>> Handle(DeleteBusinessDocumentTypeCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Deleting document type with ID: {command.Id}");

            var documentType = await _documentTypeRepository.GetByIdAsync(command.Id);
            if (documentType == null)
            {
                _logger.LogWarn($"Document type not found: ID {command.Id}");
                return Errors.BusinessDocumentType.NotFound;
            }

            //Check if in use
            if (await _documentTypeRepository.IsInUseAsync(command.Id))
            {
                _logger.LogWarn($"Document type in use: ID {command.Id}");
                return Errors.BusinessDocumentType.InUse;
            }

            await _documentTypeRepository.DeleteAsync(command.Id);
            _logger.LogInfo($"Deleted document type: {documentType.Name} (ID: {command.Id})");

            return Result.Deleted;
        }
    }
}
