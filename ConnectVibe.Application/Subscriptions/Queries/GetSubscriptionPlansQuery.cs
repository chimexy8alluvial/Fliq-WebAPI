using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Subscriptions.Queries
{
    public record GetSubscriptionPlansQuery() : IRequest<ErrorOr<List<SubscriptionPlanDto>>>;

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

        public GetSubscriptionPlansQueryHandler(ISubscriptionPlanRepository subscriptionPlanRepository, IMapper mapper)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _mapper = mapper;
        }

        public async Task<ErrorOr<List<SubscriptionPlanDto>>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {
            var plans = await _subscriptionPlanRepository.GetAllAsync();

            if (!plans.Any())
            {
                return new List<SubscriptionPlanDto>();
            }

            var result = _mapper.Map<List<SubscriptionPlanDto>>(plans);
            return result;
        }
    }

}
