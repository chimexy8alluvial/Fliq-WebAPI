using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetRecommendedEventsQuery(int UserId, int Count = 10) : IRequest<ErrorOr<List<Events>>>;

    public class GetRecommendedEventsQueryHandler : IRequestHandler<GetRecommendedEventsQuery, ErrorOr<List<Events>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IEventRepository _eventRepository;
        public GetRecommendedEventsQueryHandler(IUserRepository userRepository, IRecommendationCalculator recommendationCalculator, IRecommendationRepository recommendationRepository, IEventRepository eventRepository)
        {
            _userRepository = userRepository;
            _recommendationCalculator = recommendationCalculator;
            _recommendationRepository = recommendationRepository;
            _eventRepository = eventRepository;
        }

        public async Task<ErrorOr<List<Events>>> Handle(GetRecommendedEventsQuery request, CancellationToken cancellationToken)
        {
            var user =  _userRepository.GetUserByIdIncludingProfile(request.UserId);
            if (user == null)
            {
                return new List<Events>();
            }
            // Get user's past event interactions
            var userEventInteractions =  await _recommendationRepository.GetPastUserInteractionsAsync(request.UserId, "event");

            // Get events matching basic criteria (not past, appropriate age range, etc.)

            var userAge = CalculateAge(user.UserProfile.DOB);

            var candidateEvents = await _eventRepository.GetUpcomingByAgeRange(userAge);

            // Calculate scores and rank
            var scoredEvents = candidateEvents
                .Select(e => new {
                    Event = e,
                    Score = _recommendationCalculator.CalculateEventScore(e, user, userEventInteractions.ToList())
                })
                .OrderByDescending(item => item.Score)
                .Take(request.Count)
                .Select(item => item.Event)
                .ToList();

            return scoredEvents;
        }

        private int CalculateAge(DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
