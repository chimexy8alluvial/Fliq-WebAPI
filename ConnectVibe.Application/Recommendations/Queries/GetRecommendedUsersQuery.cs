using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Recommendations;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using MediatR;

namespace Fliq.Application.Recommendations.Queries
{
    public record GetRecommendedUsersQuery(int UserId, int Count = 10) : IRequest<ErrorOr<List<User>>>;

    public class GetRecommendedUsersQueryHandler : IRequestHandler<GetRecommendedUsersQuery, ErrorOr<List<User>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationCalculator _recommendationCalculator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _logger;

        public GetRecommendedUsersQueryHandler(
            IUserRepository userRepository,
            IRecommendationCalculator recommendationCalculator,
            IRecommendationRepository recommendationRepository,
            ILoggerManager logger)
        {
            _userRepository = userRepository;
            _recommendationCalculator = recommendationCalculator;
            _recommendationRepository = recommendationRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<User>>> Handle(GetRecommendedUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Handling GetRecommendedUsersQuery for UserId: {request.UserId}");

            var currentUser = _userRepository.GetUserByIdIncludingProfile(request.UserId);
            if (currentUser == null)
            {
                _logger.LogError($"User with UserId: {request.UserId} not found.");
                return new List<User>();
            }

            _logger.LogInfo($"Fetching past user interactions for UserId: {request.UserId}");
            var userInteractions = await _recommendationRepository.GetPastUserInteractionsAsync(request.UserId, "user");

            _logger.LogInfo($"Fetching candidate users for UserId: {request.UserId}");
            var candidateUsers = _userRepository.GetAllUsers() //Get potential matches with basic minimum filtering criteria
                .Where(u => u.Id != request.UserId)
                .ToList();

            if (!candidateUsers.Any())
            {
                _logger.LogWarn($"No candidate users found for UserId: {request.UserId}");
                return new List<User>();
            }

            _logger.LogInfo($"Calculating match scores for users for UserId: {request.UserId}");
            var scoredUsers = candidateUsers
                .Select(u => new
                {
                    User = u,
                    Score = _recommendationCalculator.CalculateUserMatchScore(u, currentUser, userInteractions.ToList())
                })
                .OrderByDescending(item => item.Score)
                .Take(request.Count)
                .Select(item => item.User)
                .ToList();

            _logger.LogInfo($"Returning {scoredUsers.Count} recommended users for UserId: {request.UserId}");
            return scoredUsers;
        }
    }
}
