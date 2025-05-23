using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Enums.Recommendations;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetCachedRecommendationsQuery(
     int UserId,
     RecommendationType RecommendationType,
     int Count = 10
    ) : IRequest<ErrorOr<object>>;

    public class GetCachedRecommendationsQueryHandler : IRequestHandler<GetCachedRecommendationsQuery, ErrorOr<object>>
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _logger;

        public GetCachedRecommendationsQueryHandler(
            IRecommendationRepository recommendationRepository,
            ILoggerManager logger)
        {
            _recommendationRepository = recommendationRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<object>> Handle(GetCachedRecommendationsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting cached {request.RecommendationType} recommendations for user {request.UserId}");

            try
            {
                return request.RecommendationType switch
                {
                    RecommendationType.Event => await GetCachedEvents(request.UserId, request.Count),
                    RecommendationType.BlindDate => await GetCachedBlindDates(request.UserId, request.Count),
                    RecommendationType.SpeedDate => await GetCachedSpeedDates(request.UserId, request.Count),
                    RecommendationType.User => await GetCachedUsers(request.UserId, request.Count),
                    _ => throw new ArgumentException("Invalid recommendation type")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting cached recommendations: {ex.Message}");
                throw new Exception("Error getting cached recommendations", ex);
            }
        }

        private async Task<List<Events>> GetCachedEvents(int userId, int count)
        {
            var cachedRecommendations = await _recommendationRepository.GetCachedRecommendationsAsync(
                userId, RecommendationType.Event, count);

            return cachedRecommendations
                .Where(cr => cr.Event != null)
                .Select(cr => cr.Event!)
                .ToList();
        }

        private async Task<List<BlindDate>> GetCachedBlindDates(int userId, int count)
        {
            var cachedRecommendations = await _recommendationRepository.GetCachedRecommendationsAsync(
                userId, RecommendationType.BlindDate, count);

            return cachedRecommendations
                .Where(cr => cr.BlindDate != null)
                .Select(cr => cr.BlindDate!)
                .ToList();
        }

        private async Task<List<SpeedDatingEvent>> GetCachedSpeedDates(int userId, int count)
        {
            var cachedRecommendations = await _recommendationRepository.GetCachedRecommendationsAsync(
                userId, RecommendationType.SpeedDate, count);

            return cachedRecommendations
                .Where(cr => cr.SpeedDatingEvent != null)
                .Select(cr => cr.SpeedDatingEvent!)
                .ToList();
        }

        private async Task<List<User>> GetCachedUsers(int userId, int count)
        {
            var cachedRecommendations = await _recommendationRepository.GetCachedRecommendationsAsync(
                userId, RecommendationType.User, count);

            return cachedRecommendations
                .Where(cr => cr.RecommendedUser != null)
                .Select(cr => cr.RecommendedUser!)
                .ToList();
        }
    }
}
