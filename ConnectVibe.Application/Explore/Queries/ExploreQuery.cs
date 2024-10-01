using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fliq.Application.Explore.Queries
{
    public record ExploreQuery(
        bool? FilterByEvent = null,
        bool? FilterByDating = null,
        bool? FilterByFriendship = null,
        int PageNumber = 1,
        int PageSize = 5
        ) : IRequest<ErrorOr<ExploreResult>>;

    public class ExploreQueryHandler : IRequestHandler<ExploreQuery, ErrorOr<ExploreResult>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private const int UnauthorizedUserId = -1;
        private readonly ILoggerManager _logger;
        //private readonly IEventRepository _eventRepository;

        public ExploreQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IProfileRepository profileRepository, ILoggerManager logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<ExploreResult>> Handle(ExploreQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Starting ExploreQuery handling for user.");

            // Get logged-in user
            var user = GetUser();
            if(user == null)
            {
                _logger.LogWarn("User not found");
                return Errors.User.UserNotFound;
            }

            if(user.UserProfile == null)
            {
                _logger.LogWarn($"UserProfile not found for user {user.Id}");
                return Errors.Profile.ProfileNotFound;
            }

            // Fetch user profiles based on filters
            _logger.LogInfo($"Fetching profiles for user --> {user.Id}");
            var profiles = await _profileRepository.GetProfilesAsync(user.Id, query.PageNumber, query.PageSize, query.FilterByDating, query.FilterByFriendship);
            _logger.LogInfo($"Successfully fetched {profiles.Count()} profiles for user.");
            var totalCount = profiles.Count();

            var paginatedProfiles = new PaginationResponse<UserProfile>(profiles, totalCount, query.PageNumber, query.PageSize);
            
            //Fetch events if the user is exploring events
            #region Fetch Events
            
            //IEnumerable<Event> events = new();
            //if (user.UserProfile.ProfileTypes.Contains(ProfileType.Events))
            //{
            //    //if(query.FilterByEvent == true)
            //    //{
            //    //    events = await _eventRepository.GetEventsAsync();
            //    //}
            //}
            #endregion

            return new ExploreResult(paginatedProfiles);
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }

        private User? GetUser()
        {
            _logger.LogInfo("Fetch logged-in user data.");
            var userId = GetUserId();
            var user = _userRepository.GetUserById(userId);
            return user;
        }
    }

    
}
