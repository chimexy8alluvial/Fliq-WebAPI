using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Infrastructure.Persistence.Repositories;
using MediatR;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetRecommendedSpeedDatesQuery(int UserId, int Count = 10) : IRequest<ErrorOr<List<SpeedDatingEvent>>>;

    public class GetRecommendedSpeedDatesQueryHandler : IRequestHandler<GetRecommendedSpeedDatesQuery, ErrorOr<List<SpeedDatingEvent>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ISpeedDatingEventRepository _speedDatingEventRepository;
        private readonly ILoggerManager _logger;

        public GetRecommendedSpeedDatesQueryHandler(
            IUserRepository userRepository,
            IRecommendationCalculator recommendationCalculator,
            IRecommendationRepository recommendationRepository,
            ISpeedDatingEventRepository speedDatingEventRepository,
            ILoggerManager logger)
        {
            _userRepository = userRepository;
            _recommendationCalculator = recommendationCalculator;
            _recommendationRepository = recommendationRepository;
            _speedDatingEventRepository = speedDatingEventRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<SpeedDatingEvent>>> Handle(GetRecommendedSpeedDatesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Handling GetRecommendedSpeedDatesQuery for UserId: {request.UserId}");

            var user = _userRepository.GetUserByIdIncludingProfile(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with UserId: {request.UserId} not found.");
                return new List<SpeedDatingEvent>();
            }

            _logger.LogInfo($"Fetching past speed dating interactions for UserId: {request.UserId}");
            var userSpeedDateInteractions = await _recommendationRepository.GetPastUserInteractionsAsync(request.UserId, "speeddate");

            _logger.LogInfo($"Fetching upcoming speed dating events for UserId: {request.UserId}");
            var candidateSpeedDates = await _speedDatingEventRepository.GetUpcomingSpeedDatingEvents();

            if (!candidateSpeedDates.Any())
            {
                _logger.LogWarn($"No upcoming speed dating events found for UserId: {request.UserId}");
                return new List<SpeedDatingEvent>();
            }

            _logger.LogInfo($"Calculating scores for speed dating events for UserId: {request.UserId}");
            var scoredSpeedDates = candidateSpeedDates
                .Select(sd => new
                {
                    SpeedDate = sd,
                    Score = _recommendationCalculator.CalculateSpeedDateScore(sd, user, userSpeedDateInteractions.ToList())
                })
                .OrderByDescending(item => item.Score)
                .Take(request.Count)
                .Select(item => item.SpeedDate)
                .ToList();

            _logger.LogInfo($"Returning {scoredSpeedDates.Count} recommended speed dating events for UserId: {request.UserId}");
            return scoredSpeedDates;
        }
    }
}
