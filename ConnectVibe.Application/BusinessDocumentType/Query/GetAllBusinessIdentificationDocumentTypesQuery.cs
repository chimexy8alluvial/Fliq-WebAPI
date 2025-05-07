using ErrorOr;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Mapster;
using MediatR;

namespace Fliq.Application.BusinessDocumentType.Query
{
    public record GetAllBusinessIdentificationDocumentTypesQuery : IRequest<ErrorOr<List<BusinessIdentificationDocumentTypeResponse>>>;
    public class GetAllBusinessIdentificationDocumentTypesQueryHandler : IRequestHandler<GetAllBusinessIdentificationDocumentTypesQuery, ErrorOr<List<BusinessIdentificationDocumentTypeResponse>>>
    {
        private readonly IBusinessIdentificationDocumentTypeRepository _documentRepository;
        private readonly ILoggerManager _logger;

        public GetAllBusinessIdentificationDocumentTypesQueryHandler(IBusinessIdentificationDocumentTypeRepository documentRepository, ILoggerManager logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<BusinessIdentificationDocumentTypeResponse>>> Handle(GetAllBusinessIdentificationDocumentTypesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Retrieving all business document types");

            var documentTypes = _documentRepository.GetAllBusinessIdentificationDocumentTypesAsync();
            var result = documentTypes.Adapt<List<BusinessIdentificationDocumentTypeResponse>>();

            _logger.LogInfo($"Retrieved {result.Count} document types");
            return result;
        }
    }
}
