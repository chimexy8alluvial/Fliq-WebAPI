using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.InActiveUserCount
{
    public record GetInActiveUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetInActiveUsersCountQueryHandler : IRequestHandler<GetInActiveUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetInActiveUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetInActiveUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching Inactive users count...");

            var count = await _userRepository.CountInactiveUsers();
            _logger.LogInfo($"Inactive Users Count: {count}");

            return new CountResult(count);
        }
    }

}
