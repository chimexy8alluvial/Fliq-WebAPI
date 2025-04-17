

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetNumberOfGamersCountQuery
{
    public record GetNumberOfGamersCountQuery() : IRequest<ErrorOr<CountResult>>;
        public class GetNumberOfGamersCountQueryHandler : IRequestHandler<GetNumberOfGamersCountQuery, ErrorOr<CountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;
            public GetNumberOfGamersCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }
            public async Task<ErrorOr<CountResult>> Handle(GetNumberOfGamersCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all gamers count ...");
                var count = await _gamesRepository.GetGamersCountAsync();
                _logger.LogInfo($"All gamers count: {count}");

                return new CountResult(count);
            }
        }

}
