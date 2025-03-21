
using ErrorOr;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.DatingEnvironment.Queries.DatingDashboard
{
    public record SpeedDateCountQuery() : IRequest<ErrorOr<UserCountResult>>;
    public class SpeedDateCountQueryHandler : IRequestHandler <SpeedDateCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly ILoggerManager _logger;
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;
        public SpeedDateCountQueryHandler(ILoggerManager logger, ISpeedDatingEventRepository speedDatingEventRepository)
        {
            _speedDatingEventRepository = speedDatingEventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(SpeedDateCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all speed date event count...");

            var count = await _speedDatingEventRepository.GetSpeedDateCountAsync();
            _logger.LogInfo($"All speed date event count: {count}");

            return new UserCountResult(count);
        }
    }
}

