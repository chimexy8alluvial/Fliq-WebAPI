using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Users.Common;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;


namespace Fliq.Application.Users.Queries
{
    public record GetRecentUserFeatureActivitiesQuery(int AdminUserId, int UserId, int Limit) : IRequest<ErrorOr<List<GetRecentUserFeatureActivityResult>>>;

    public class GetRecentUserFeatureActivitiesQueryHandler : IRequestHandler<GetRecentUserFeatureActivitiesQuery, ErrorOr<List<GetRecentUserFeatureActivityResult>>>
    {
        private readonly IUserFeatureActivityRepository _userFeatureActivityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public GetRecentUserFeatureActivitiesQueryHandler(IUserFeatureActivityRepository userFeatureActivityRepository, IUserRepository userRepository, ILoggerManager logger, IMapper mapper)
        {
            _userFeatureActivityRepository = userFeatureActivityRepository;
            _userRepository = userRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ErrorOr<List<GetRecentUserFeatureActivityResult>>> Handle(GetRecentUserFeatureActivitiesQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Starting process to Get Recent Feature Activities for User with ID {query.UserId}");

            var adminUser = _userRepository.GetUserById(query.AdminUserId);
            if(adminUser == null)
            {
                _logger.LogError($"Admin user with ID {query.AdminUserId} not found");
                return Errors.User.UserNotFound;
            }

            var featureUser = _userRepository.GetUserById(query.UserId);
            if(featureUser == null)
            {
                _logger.LogError($"user with ID {query.UserId} not found");
                return Errors.User.UserNotFound;
            }

            // Validate and enforce max limit
            var limit = Math.Min(query.Limit, 10);

            _logger.LogInfo($"Fetching {limit} Recent Feature Activities for User with ID {query.UserId}");
            var recentFeatureActivities =  _mapper.Map<List<GetRecentUserFeatureActivityResult>>( await _userFeatureActivityRepository.GetRecentUserFeatureActivitiesAsync(query.UserId, limit));
            _logger.LogInfo($"{recentFeatureActivities.Count}Recent Feature Activities for User with ID {query.UserId}");

            return recentFeatureActivities;

        }
    }
}
