using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Profile;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Fliq.Application.Explore.Queries
{
    public record ExploreQuery(
        int UserId,
        bool? FilterByEvent = null,
        bool? FilterByDating = null,
        bool? FilterByFriendship = null,
        PaginationRequest PaginationRequest = default!
        ) : IRequest<ErrorOr<ExploreResult>>;

    public class ExploreQueryHandler : IRequestHandler<ExploreQuery, ErrorOr<ExploreResult>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IProfileMatchingService _profileMatchingService;
        private const int UnauthorizedUserId = -1;
        private readonly ILoggerManager _logger;

        public ExploreQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IProfileRepository profileRepository, ILoggerManager logger, IProfileMatchingService profileMatchingService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _logger = logger;
            _profileMatchingService = profileMatchingService;
        }

        public async Task<ErrorOr<ExploreResult>> Handle(ExploreQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting ExploreQuery handling for user.");

            // Get logged-in user
            var user = _userRepository.GetUserById(query.UserId);
            if (user == null)
            {
                _logger.LogWarn("User not found");
                return Errors.User.UserNotFound;
            }

            var userProfile = _profileRepository.GetProfileByUserId(query.UserId);

            if (userProfile == null)
            {
                _logger.LogWarn($"UserProfile not found for user {user.Id}");
                return Errors.Profile.ProfileNotFound;
            }

            // Fetch user profiles based on filters
            _logger.LogInfo($"Fetching profiles for user --> {user.Id}");
            var profiles = await _profileMatchingService.GetMatchedProfilesAsync(user, query);
            _logger.LogInfo($"Successfully fetched {profiles.Count()} profiles for user.");
            var totalCount = profiles.Count();

            var paginatedProfiles = new PaginationResponse<UserProfile>(profiles, totalCount, query.PaginationRequest.PageNumber, query.PaginationRequest.PageSize);

            return new ExploreResult(paginatedProfiles);
        }
    }

}
