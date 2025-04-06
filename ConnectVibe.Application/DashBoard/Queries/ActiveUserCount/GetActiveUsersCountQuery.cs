

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.ActiveUserCount
{
    public record GetActiveUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetActiveUsersCountQueryHandler : IRequestHandler<GetActiveUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetActiveUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetActiveUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching active users count...");

            var count = await _userRepository.CountActiveUsers();
            _logger.LogInfo($"Active Users Count: {count}");

            return new CountResult(count);
        }
    }

}
