using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Games.Queries.GetTotalGamesPlayed
{
    public class GetTotalGamesPlayedCountQuery
    {
        public record TotalGamesPlayedCountQuery() : IRequest<ErrorOr<UserCountResult>>;
        public class TotalGamesPlayedCountQueryHandler : IRequestHandler<TotalGamesPlayedCountQuery, ErrorOr<UserCountResult>>
        {
            private readonly ILoggerManager _logger;
            private readonly IGamesRepository _gamesRepository;
            public TotalGamesPlayedCountQueryHandler(ILoggerManager logger, IGamesRepository gamesRepository)
            {
                _logger = logger;
                _gamesRepository = gamesRepository;
            }

            public async Task<ErrorOr<UserCountResult>> Handle(TotalGamesPlayedCountQuery query, CancellationToken cancellationToken)
            {
                _logger.LogInfo("Fetching all total games played count ...");
                var count = await _gamesRepository.GetTotalGamesPlayedCountAsync();
                _logger.LogInfo($"All total games played count: {count}");

                return new UserCountResult(count);
            }
        }

    }
}
