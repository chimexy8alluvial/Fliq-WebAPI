
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Subscriptions;
using MediatR;

namespace Fliq.Application.Subscriptions.Commands
{
    public record AddSubscriptionPlanPriceCommand(
        int SubscriptionPlanId, 
        decimal Amount, 
        string Currency, 
        string Country,
        DateTime EffectiveFrom,
        string? Store
        ) : IRequest<ErrorOr<Unit>>;
    public class AddSubscriptionPlanPriceCommandHandler : IRequestHandler<AddSubscriptionPlanPriceCommand, ErrorOr<Unit>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly ISubscriptionPlanPriceRepository _subscriptionPlanPriceRepository;
        private readonly ILoggerManager _loggerManager;
        public AddSubscriptionPlanPriceCommandHandler(ISubscriptionPlanRepository subscriptionPlanRepository, ILoggerManager loggerManager, ISubscriptionPlanPriceRepository subscriptionPlanPriceRepository)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _loggerManager = loggerManager;
            _subscriptionPlanPriceRepository = subscriptionPlanPriceRepository;
        }
        public async Task<ErrorOr<Unit>> Handle(AddSubscriptionPlanPriceCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting price creation process for subscription plan: {request.SubscriptionPlanId}");

            var existingSubPlan = await _subscriptionPlanRepository.GetByIdAsync(request.SubscriptionPlanId);
            if (existingSubPlan == null)
            {
                _loggerManager.LogError($"No subscription plan found for ID: {request.SubscriptionPlanId}. Aborting creation.");
                return Errors.Subscription.DuplicateSubscriptionPlanPrice;
            }

            var newSubPlanPrice = new SubscriptionPlanPrice
            {
                SubscriptionPlanId = request.SubscriptionPlanId,
                Amount = request.Amount,
                Country = request.Country,
                Currency = request.Currency,
                EffectiveFrom = request.EffectiveFrom,
                Store = request.Store,
            };
            await _subscriptionPlanPriceRepository.AddAsync(newSubPlanPrice);
            _loggerManager.LogInfo($"Successfully added new price for subscription plan: {existingSubPlan.Name}");

            return Unit.Value;
        }
    }
}
