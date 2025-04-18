using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Contracts.Explore;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;

namespace Fliq.Application.Explore.Queries
{
    public record ExploreEventsQuery(
    int UserId,
    double? MaxDistanceKm = null,
    EventCategory? Category = null,
    EventType? EventType = null,
    string? CreatedBy = null,
    string? EventTitle = null,
    EventStatus? Status = null,
    bool? IncludeReviews = null,
    int? MinRating = null,
    PaginationRequest PaginationRequest = default!
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

            // Get logged-in user
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

            // Validate location
            if (user.UserProfile.Location?.LocationDetail == null)
            {
                _logger.LogWarn($"User location not found for user {user.Id}");
                return Error.Failure(code: "Profile.LocationNotFound", description: "User location is not configured.");
            }

            // Fetch events based on filters
            _logger.LogInfo($"Fetching events {(query.IncludeReviews == true ? "with reviews" : "without reviews")} for user {user.Id} with filters: Distance={query.MaxDistanceKm}, Category={query.Category}, Type={query.EventType}, CreatedBy={query.CreatedBy}, Status={query.Status}, IncludeReviews={query.IncludeReviews}, MinRating={query.MinRating}, PageNumber={query.PaginationRequest.PageNumber}, PageSize={query.PaginationRequest.PageSize}");

            try
            {
                var (events, totalCount) = await _eventRepository.GetEventsAsync(
                    userLocation: user.UserProfile.Location.LocationDetail,
                    maxDistanceKm: query.MaxDistanceKm,
                    userProfile: user.UserProfile,
                    category: query.Category,
                    eventType: query.EventType,
                    createdBy: query.CreatedBy,
                    eventTitle: query.EventTitle,
                    status: query.Status,
                    includeReviews: query.IncludeReviews,
                    minRating: query.MinRating,
                    pagination: query.PaginationRequest
                );

                var paginatedEvents = new PaginationResponse<EventWithDisplayName>(
                    events ?? new List<EventWithDisplayName>(),
                    totalCount,
                    query.PaginationRequest.PageNumber,
                    query.PaginationRequest.PageSize
                );

                _logger.LogInfo($"Fetched {totalCount} events {(query.IncludeReviews == true ? "with reviews" : "without reviews")} for user {user.Id}. Page {query.PaginationRequest.PageNumber} has {paginatedEvents.Data.Count()} items.");

                return new ExploreEventsResult(paginatedEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch events {(query.IncludeReviews == true ? "with reviews" : "without reviews")} for user {user.Id}: {ex.Message}");
                return Error.Failure(description: $"Failed to fetch events: {ex.Message}");
            }
        }
    }
}