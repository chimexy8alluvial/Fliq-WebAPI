using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.MaleUsersCount
{
    public record GetAllMaleUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllMaleUsersCountQueryHandler : IRequestHandler<GetAllMaleUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllMaleUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllMaleUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all male-users count...");

            var count = await _userRepository.CountAllMaleUsers();
            _logger.LogInfo($"All male-users count: {count}");

            return new CountResult(count);
        }
    }
}
