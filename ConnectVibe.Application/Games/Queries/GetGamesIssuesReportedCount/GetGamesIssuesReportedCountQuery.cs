
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetIssuesReportedCount
{
    public record GetGamesIssuesReportedCountQuery() : IRequest<ErrorOr<CountResult>>;
    public class GetGamesIssuesReportedCountQueryHandler : IRequestHandler<GetGamesIssuesReportedCountQuery, ErrorOr<CountResult>>
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILoggerManager _logger;
        public GetGamesIssuesReportedCountQueryHandler(IGamesRepository gamesRepository, ILoggerManager logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }
        public async Task<ErrorOr<CountResult>> Handle(GetGamesIssuesReportedCountQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all issues reported games count ...");
            var count = await _gamesRepository.GetGamesIssuesReportedCountAsync();
            _logger.LogInfo($"All issues reported games count: {count}");

            return new CountResult(count);
        }
    }
}



