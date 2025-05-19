using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using MediatR;



namespace Fliq.Application.Explore.Queries
{
    public record ExploreQuery(
        PaginationRequest PaginationRequest,
        int UserId,
        bool? FilterByEvent = null,
        bool? FilterByDating = null,
        bool? FilterByFriendship = null
        
        ) : IRequest<ErrorOr<ExploreResult>>;

    public class ExploreQueryHandler : IRequestHandler<ExploreQuery, ErrorOr<ExploreResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IProfileMatchingService _profileMatchingService;
        private readonly ILoggerManager _logger;

        public ExploreQueryHandler(IUserRepository userRepository, IProfileRepository profileRepository, ILoggerManager logger, IProfileMatchingService profileMatchingService)
        {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _logger = logger;
            _profileMatchingService = profileMatchingService;
        }

        public async Task<ErrorOr<ExploreResult>> Handle(ExploreQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting ExploreQuery handling for user.");

            var user = _userRepository.GetUserById(query.UserId);
            if (user == null)
            {
                _logger.LogWarn("User not found");
                return Errors.User.UserNotFound;
            }

            if (user.UserProfile == null)
            {
                _logger.LogWarn($"UserProfile not found for user {user.Id}");
                return Errors.Profile.ProfileNotFound;
            }

            _logger.LogInfo($"Fetching profiles for user --> {user.Id}");
            var profiles = await _profileMatchingService.GetMatchedProfilesAsync(user, query);
            _logger.LogInfo($"Successfully fetched {profiles.Data.Count()} profiles for user.");

            return new ExploreResult(profiles);
        }
    }

}
