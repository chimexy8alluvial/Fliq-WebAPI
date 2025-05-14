using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common.UnifiedSearch;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.UnifiedSearch
{
    public record UnifiedSearchQuery(string SearchTerm) : IRequest<ErrorOr<UnifiedSearchResult>>;

    public class UnifiedSearchQueryHandler : IRequestHandler<UnifiedSearchQuery, ErrorOr<UnifiedSearchResult>>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILoggerManager _logger;

        public UnifiedSearchQueryHandler(
            ISearchRepository searchRepository,
            ILoggerManager logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UnifiedSearchResult>> Handle(UnifiedSearchQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Performing unified search for term: {request.SearchTerm}");

            try
            {
                return await _searchRepository.SearchAcrossEntitiesAsync(request.SearchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in unified search: {ex.Message}");
                throw;
            }
        }
    }
}
