
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.MatchedProfile.Queries
{
    public record GetRecentUsersMatchQuery(int AdminUserId, int UserId, int Limit) : IRequest<ErrorOr<List<GetRecentUserMatchResult>>>;

    public class GetRecentUsersMatchQueryHandler : IRequestHandler<GetRecentUsersMatchQuery, ErrorOr<List<GetRecentUserMatchResult>>>
    {
        private readonly IMatchProfileRepository _matchRepository;
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;

        public GetRecentUsersMatchQueryHandler(IMatchProfileRepository matchRepository, ILoggerManager logger, IUserRepository userRepository)
        {
            _matchRepository = matchRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<List<GetRecentUserMatchResult>>> Handle(GetRecentUsersMatchQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Fetching {query.Limit} recent matches for User ID {query.UserId}");

            var adminUser = _userRepository.GetUserById(query.AdminUserId);
            if (adminUser == null)
            {
                _logger.LogError($"Admin user with ID {query.AdminUserId} not found");
                return Errors.User.UserNotFound;
            }
            if (adminUser.RoleId is not (1 or 2))
            {
                _logger.LogError($"User with ID {query.AdminUserId} is not an Admin");
                return Errors.User.UnauthorizedUser;
            }

            var user = _userRepository.GetUserById(query.UserId);
            if (user == null)
            {
                _logger.LogError($"user with ID {query.UserId} not found");
                return Errors.User.UserNotFound;
            }

            // Validate and enforce max limit
            var limit = Math.Min(query.Limit, 10);
            if (limit < query.Limit) _logger.LogInfo($"Limit for query has been reduced from {query.Limit} to {limit}");

            // Fetch recent matches using optimized query
            _logger.LogInfo($"Fetching Recent Matches...");
            var recentMatches = await _matchRepository.GetRecentMatchesAsync(query.UserId, limit);
            var recentMatchesList = recentMatches?.ToList() ?? [];
            var count = recentMatchesList.Count < limit ? recentMatchesList.Count : limit;
            _logger.LogInfo($"{count} Recent User Matches Fetched");

            return recentMatchesList;
        }
    }
}
