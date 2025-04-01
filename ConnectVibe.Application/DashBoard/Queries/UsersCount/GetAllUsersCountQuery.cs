

using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.UsersCount
{
    public record GetAllUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllUsersCountQueryHandler : IRequestHandler<GetAllUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all users count...");

            var count = await _userRepository.CountAllUsers();
            _logger.LogInfo($"All Users Count: {count}");

            return new CountResult(count);
        }
    }
}
