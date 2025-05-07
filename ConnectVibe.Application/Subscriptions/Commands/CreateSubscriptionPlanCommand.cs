using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Subscriptions;
using MediatR;

namespace Fliq.Application.Subscriptions.Commands
{
    public record CreateSubscriptionPlanCommand(string Name, string ProductId, string? Description) : IRequest<ErrorOr<Unit>>;

    public class CreateSubscriptionPlanCommandHandler : IRequestHandler<CreateSubscriptionPlanCommand, ErrorOr<Unit>>
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly ILoggerManager _loggerManager;
        public CreateSubscriptionPlanCommandHandler(ISubscriptionPlanRepository subscriptionPlanRepository, ILoggerManager loggerManager)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _loggerManager = loggerManager;
        }
        public async Task<ErrorOr<Unit>> Handle(CreateSubscriptionPlanCommand request, CancellationToken cancellationToken)
        {
            _loggerManager.LogInfo($"Starting subscription plan creation process for name: {request.Name}");

            var existingSubPlan = await _subscriptionPlanRepository.GetByName(request.Name);
            if (existingSubPlan != null)
            {
                _loggerManager.LogWarn($"Duplicate subscription plan detected: {request.Name}. Aborting creation.");
                return Errors.Subscription.DuplicateSubscriptionPlan;
            }

            var newSubPlan = new SubscriptionPlan
            {
                Name = request.Name,
                ProductId = request.ProductId,
                Description = request.Description,
            };
            await _subscriptionPlanRepository.AddAsync(newSubPlan);
            _loggerManager.LogInfo($"Successfully added new subscription plan: {newSubPlan.Name} with ID: {newSubPlan.Id}");

            return Unit.Value;
        }
    }
}
