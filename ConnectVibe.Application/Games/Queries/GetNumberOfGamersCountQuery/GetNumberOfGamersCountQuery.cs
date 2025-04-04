

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.Games.Queries.GetNumberOfGamersCountQuery
{
    public class GetNumberOfGamersCountQuery
    {
        public record NumberOfGamersCountQuery() : IRequest<ErrorOr<UserCountResult>>;
        public class NumberOfGamersCountQueryHandler : IRequestHandler<NumberOfGamersCountQuery, ErrorOr<UserCountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;
            public NumberOfGamersCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }
            public async Task<ErrorOr<UserCountResult>> Handle(NumberOfGamersCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all gamers count ...");
                var count = await _gamesRepository.GetGamersCountAsync();
                _logger.LogInfo($"All gamers count: {count}");

                return new UserCountResult(count);
            }
        }

    }
}
