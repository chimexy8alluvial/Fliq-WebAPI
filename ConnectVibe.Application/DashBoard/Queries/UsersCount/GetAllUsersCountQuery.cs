

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.UsersCount
{
    public record GetAllUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetAllUsersCountQueryHandler : IRequestHandler<GetAllUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetAllUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all users count...");

            var count = await _userRepository.CountAllUsers();
            _logger.LogInfo($"All Users Count: {count}");

            return new UserCountResult(count);
        }
    }
}
