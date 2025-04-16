using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Explore.Queries
{
    public record ExploreEventsQuery(
        int UserId,
        double? MaxDistanceKm = null,
        EventCategory? Category = null,
        EventType? EventType = null,
        int? CreatorId = null,
        EventStatus? Status= null,
        int PageNumber = 1,
        int PageSize = 5
    ) : IRequest<ErrorOr<ExploreEventsResult>>;

    public class ExploreEventsQueryHandler : IRequestHandler<ExploreEventsQuery, ErrorOr<ExploreEventsResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public ExploreEventsQueryHandler(
            IUserRepository userRepository,
            IEventRepository eventRepository,
            ILoggerManager logger)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<ExploreEventsResult>> Handle(ExploreEventsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Starting ExploreEventsQuery handling for user {query.UserId}.");

            // Validate user, profile, and location
            var validationResult = ValidateUserProfileAndLocation(query.UserId);
            if (validationResult.IsError)
            {
                return validationResult.Errors;
            }

            var user = validationResult.Value;

            // Fetch events based on filters
            _logger.LogInfo($"Fetching events for user {user.Id} with filters: Distance={query.MaxDistanceKm}, Category={query.Category}, Type={query.EventType}, Creator={query.CreatorId}, Status={query.Status}");

            try
            {
                var (events, totalCount) = await _eventRepository.GetEventsAsync(
                    userLocation: user.UserProfile!.Location!.LocationDetail,
                    maxDistanceKm: query.MaxDistanceKm,
                    userProfile: user.UserProfile,
                    category: query.Category,
                    eventType: query.EventType,
                    creatorId: query.CreatorId,
                    status: query.Status,
                    pagination: new PaginationRequest(query.PageNumber, query.PageSize)
                );

                // Ensure events is never null
                var eventList = events?.ToList() ?? new List<Events>();
                var paginatedEvents = new PaginationResponse<Events>(
                    eventList,
                    totalCount,
                    query.PageNumber,
                    query.PageSize
                );

                _logger.LogInfo($"Fetched {totalCount} events for user {user.Id}. Page {query.PageNumber} has {eventList.Count} items.");

                return new ExploreEventsResult(paginatedEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch events for user {user.Id}: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return Error.Failure(description: $"Failed to fetch events: {ex.Message}");
            }
        }

        private ErrorOr<User> ValidateUserProfileAndLocation(int userId)
        {
            var user = _userRepository.GetUserById(userId);
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

            if (user.UserProfile.Location?.LocationDetail == null)
            {
                _logger.LogWarn($"User location not found for user {user.Id}");
                return Error.Failure(code: "Profile.LocationNotFound", description: "User location is not configured.");
            }

            return user;
        }
    }
}