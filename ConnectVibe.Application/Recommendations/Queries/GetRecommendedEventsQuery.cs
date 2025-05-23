using ErrorOr;
using Fliq.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Common;
using Fliq.Domain.Entities.Event;
using MediatR;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetRecommendedEventsQuery(int UserId, int Count = 10) : IRequest<ErrorOr<List<ScoredEventRecommendation>>>;

    public class GetRecommendedEventsQueryHandler : IRequestHandler<GetRecommendedEventsQuery, ErrorOr<List<ScoredEventRecommendation>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILoggerManager _logger;

        public GetRecommendedEventsQueryHandler(IUserRepository userRepository, IRecommendationCalculator recommendationCalculator, IRecommendationRepository recommendationRepository, IEventRepository eventRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _recommendationCalculator = recommendationCalculator;
            _recommendationRepository = recommendationRepository;
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<ScoredEventRecommendation>>> Handle(GetRecommendedEventsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Handling GetRecommendedEventsQuery for UserId: {request.UserId}");

            var user = _userRepository.GetUserByIdIncludingProfile(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with UserId: {request.UserId} not found.");
                return new List<ScoredEventRecommendation>();
            }

            _logger.LogInfo($"Fetching past event interactions for UserId: {request.UserId}");
            var userEventInteractions = await _recommendationRepository.GetPastUserInteractionsAsync(request.UserId, "event");

            _logger.LogInfo($"Calculating age for UserId: {request.UserId}");
            var userAge = Extensions.CalculateAge(user.UserProfile.DOB);

            _logger.LogInfo($"Fetching upcoming events for UserId: {request.UserId} with age: {userAge}");
            var candidateEvents = await _eventRepository.GetUpcomingByAgeRange(userAge);

            if (!candidateEvents.Any())
            {
                _logger.LogWarn($"No upcoming events found for UserId: {request.UserId} with age: {userAge}");
                return new List<ScoredEventRecommendation>();
            }

            _logger.LogInfo($"Calculating scores for events for UserId: {request.UserId}");
            var scoredEvents = candidateEvents
                .Select(e => new
                {
                    Event = e,
                    Score = _recommendationCalculator.CalculateEventScore(e, user, userEventInteractions.ToList())
                })
                .OrderByDescending(item => item.Score)
                .Take(request.Count)
                .Select(item => new ScoredEventRecommendation(item.Event, item.Score))
                .ToList();

            _logger.LogInfo($"Returning {scoredEvents.Count} recommended events for UserId: {request.UserId}");
            return scoredEvents;
        }
    }
}
