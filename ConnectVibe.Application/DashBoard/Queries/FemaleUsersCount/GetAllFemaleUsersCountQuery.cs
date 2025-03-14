using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.FemaleUsersCount
{

    public record GetAllFemaleUsersCountQuery() : IRequest<ErrorOr<CountResult>>;

    public class GetAllFemaleUsersCountQueryHandler : IRequestHandler<GetAllFemaleUsersCountQuery, ErrorOr<CountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllFemaleUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CountResult>> Handle(GetAllFemaleUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all female-users count...");

            var count = await _userRepository.CountAllFemaleUsers();
            _logger.LogInfo($"All female-users count: {count}");

            return new CountResult(count);
        }
    }
}
