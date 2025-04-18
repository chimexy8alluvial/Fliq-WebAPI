using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;

namespace Fliq.Application.BusinessDocumentType.Query
{
    public record GetAllBusinessDocumentTypesQuery : IRequest<ErrorOr<List<BusinessDocumentTypeResponse>>>;
    public class GetAllBusinessDocumentTypesQueryHandler : IRequestHandler<GetAllBusinessDocumentTypesQuery, ErrorOr<List<BusinessDocumentTypeResponse>>>
    {
        private readonly IBusinessDocumentTypeRepository _documentRepository;
        private readonly ILoggerManager _logger;

        public GetAllBusinessDocumentTypesQueryHandler(IBusinessDocumentTypeRepository documentRepository, ILoggerManager logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<BusinessDocumentTypeResponse>>> Handle(GetAllBusinessDocumentTypesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Retrieving all business document types");

            var documentTypes = _documentRepository.GetAllBusinessDocumentTypesAsync();
            var result = documentTypes.Adapt<List<BusinessDocumentTypeResponse>>();

            _logger.LogInfo($"Retrieved {result.Count} document types");
            return result;
        }
    }
}
