using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;
using Fliq.Domain.Common.Errors;

namespace Fliq.Application.BusinessDocumentType.Query
{
    public record GetBusinessIdentificationDocumentTypeByIdQuery(int Id) : IRequest<ErrorOr<BusinessIdentificationDocumentTypeResponse>>;

    public class GetBusinessIdentificationDocumentTypeByIdQueryHandler : IRequestHandler<GetBusinessIdentificationDocumentTypeByIdQuery, ErrorOr<BusinessIdentificationDocumentTypeResponse>>
    {
        private readonly IBusinessIdentificationDocumentTypeRepository _documentTypeRepository;
        private readonly ILoggerManager _logger;

        public GetBusinessIdentificationDocumentTypeByIdQueryHandler(
            IBusinessIdentificationDocumentTypeRepository documentTypeRepository,
            ILoggerManager logger)
        {
            _documentTypeRepository = documentTypeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<BusinessIdentificationDocumentTypeResponse>> Handle(GetBusinessIdentificationDocumentTypeByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Retrieving document type with ID: {query.Id}");

            var documentType = await _documentTypeRepository.GetByIdAsync(query.Id);
            if (documentType == null)
            {
                _logger.LogWarn($"Document type not found: ID {query.Id}");
                return Errors.BusinessDocumentType.NotFound;
            }

            var result = documentType.Adapt<BusinessIdentificationDocumentTypeResponse>();
            _logger.LogInfo($"Retrieved document type: {result.Name} (ID: {result.Id})");

            return result;
        }
    }
}
