

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.ActiveUserCount
{
    public record GetActiveUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetActiveUsersCountQueryHandler : IRequestHandler<GetActiveUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetActiveUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetActiveUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching active users count...");

            var count = await _userRepository.CountActiveUsers();
            _logger.LogInfo($"Active Users Count: {count}");

            return new UserCountResult(count);
        }
    }

}
