using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fliq.Application.Subscriptions.Queries
{
    public record GetSubscriptionPlansQuery(int UserId) : IRequest<ErrorOr<List<SubscriptionPlanDto>>>;

    public class SubscriptionPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductId { get; set; }
        public string? Description { get; set; }
    }

    public class GetSubscriptionPlansQueryHandler : IRequestHandler<GetSubscriptionPlansQuery, ErrorOr<List<SubscriptionPlanDto>>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetSubscriptionPlansQueryHandler(ISubscriptionPlanRepository subscriptionPlanRepository, IMapper mapper, IUserRepository userRepository, ILoggerManager logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<List<SubscriptionPlanDto>>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {

            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with id {request.UserId} not found.");
                return Errors.User.UserNotFound;
            }

            var plans = await _subscriptionPlanRepository.GetAllAsync();

            if (!plans.Any())
            {
                return new List<SubscriptionPlanDto>();
            }

            _logger.LogInfo($"{plans.Count} subscription plans found");
            var result = _mapper.Map<List<SubscriptionPlanDto>>(plans);
            return result;
        }
    }

}
