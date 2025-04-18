using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.BusinessDocumentType.Query
{
    public record GetBusinessDocumentTypeByIdQuery(int Id) : IRequest<ErrorOr<BusinessDocumentTypeResponse>>;

    public class GetBusinessDocumentTypeByIdQueryHandler : IRequestHandler<GetBusinessDocumentTypeByIdQuery, ErrorOr<BusinessDocumentTypeResponse>>
    {
        private readonly IBusinessDocumentTypeRepository _documentTypeRepository;
        private readonly ILoggerManager _logger;

        public GetBusinessDocumentTypeByIdQueryHandler(
            IBusinessDocumentTypeRepository documentTypeRepository,
            ILoggerManager logger)
        {
            _documentTypeRepository = documentTypeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<BusinessDocumentTypeResponse>> Handle(GetBusinessDocumentTypeByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Retrieving document type with ID: {query.Id}");

            var documentType = await _documentTypeRepository.GetByIdAsync(query.Id);
            if (documentType == null)
            {
                _logger.LogWarn($"Document type not found: ID {query.Id}");
                return Errors.BusinessDocumentType.NotFound;
            }

            var result = documentType.Adapt<BusinessDocumentTypeResponse>();
            _logger.LogInfo($"Retrieved document type: {result.Name} (ID: {result.Id})");

            return result;
        }
    }
}
