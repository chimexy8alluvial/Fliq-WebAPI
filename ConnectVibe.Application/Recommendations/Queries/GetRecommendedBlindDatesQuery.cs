using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using MediatR;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetRecommendedBlindDatesQuery(int UserId, int Count = 10) : IRequest<ErrorOr<List<BlindDate>>>;

    public class GetRecommendedBlindDatesQueryHandler : IRequestHandler<GetRecommendedBlindDatesQuery, ErrorOr<List<BlindDate>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IBlindDateRepository _blindDateRepository;
        private readonly ILoggerManager _logger;

        public GetRecommendedBlindDatesQueryHandler(
            IUserRepository userRepository,
            IRecommendationCalculator recommendationCalculator,
            IRecommendationRepository recommendationRepository,
            IBlindDateRepository blindDateRepository,
            ILoggerManager logger)
        {
            _userRepository = userRepository;
            _recommendationCalculator = recommendationCalculator;
            _recommendationRepository = recommendationRepository;
            _blindDateRepository = blindDateRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<BlindDate>>> Handle(GetRecommendedBlindDatesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Handling GetRecommendedBlindDatesQuery for UserId: {request.UserId}");

            var user = _userRepository.GetUserByIdIncludingProfile(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with UserId: {request.UserId} not found.");
                return new List<BlindDate>();
            }

            _logger.LogInfo($"Fetching past blind date interactions for UserId: {request.UserId}");
            var userBlindDateInteractions = await _recommendationRepository.GetPastUserInteractionsAsync(request.UserId, "blinddate");

            _logger.LogInfo($"Fetching upcoming blind dates for UserId: {request.UserId}");
            var candidateBlindDates = await _blindDateRepository.GetUpcomingBlindDates();

            if (!candidateBlindDates.Any())
            {
                _logger.LogWarn($"No upcoming blind dates found for UserId: {request.UserId}");
                return new List<BlindDate>();
            }

            _logger.LogInfo($"Calculating scores for blind dates for UserId: {request.UserId}");
            var scoredBlindDates = candidateBlindDates
                .Select(bd => new
                {
                    BlindDate = bd,
                    Score = _recommendationCalculator.CalculateBlindDateScore(bd, user, userBlindDateInteractions.ToList())
                })
                .OrderByDescending(item => item.Score)
                .Take(request.Count)
                .Select(item => item.BlindDate)
                .ToList();

            _logger.LogInfo($"Returning {scoredBlindDates.Count} recommended blind dates for UserId: {request.UserId}");
            return scoredBlindDates;
        }
    }
}
