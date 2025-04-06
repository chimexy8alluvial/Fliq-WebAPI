using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.OtherUsersCount
{
    public record GetAllOtherUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllOtherUsersCountQueryHandler : IRequestHandler<GetAllOtherUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllOtherUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllOtherUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all other-users count...");

            var count = await _userRepository.CountAllOtherUsers();
            _logger.LogInfo($"All other-users count: {count}");

            return new CountResult(count);
        }
    }
}
