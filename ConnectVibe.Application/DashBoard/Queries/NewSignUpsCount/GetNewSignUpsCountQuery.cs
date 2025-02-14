
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;

namespace Fliq.Application.DashBoard.Queries.NewSignUpsCount
{
    public record GetNewSignUpsCountQuery(int Days) : IRequest<ErrorOr<UserCountResult>>;

    public class GetNewSignUpsCountQueryHandler : IRequestHandler<GetNewSignUpsCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetNewSignUpsCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetNewSignUpsCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Fetching new signups count in the last {query.Days} days...");

            var count = await _userRepository.CountNewSignups(query.Days);
            _logger.LogInfo($"New Signups Count: {count}");

            return new UserCountResult(count);
        }
    }

}
