

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.InActiveUserCount
{
    public record GetInActiveUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetInActiveUsersCountQueryHandler : IRequestHandler<GetInActiveUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetInActiveUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetInActiveUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching Inactive users count...");

            var count = await _userRepository.CountInactiveUsers();
            _logger.LogInfo($"Inactive Users Count: {count}");

            return new UserCountResult(count);
        }
    }

}
