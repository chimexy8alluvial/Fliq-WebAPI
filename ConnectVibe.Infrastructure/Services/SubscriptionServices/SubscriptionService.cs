using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Persistence.Subscriptions;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Subscriptions;
using Environment = Fliq.Domain.Entities.Environment;

namespace Fliq.Infrastructure.Services.SubscriptionServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ILoggerManager _logger;

        public SubscriptionService(ILoggerManager logger, ISubscriptionPlanRepository subscriptionPlanRepository, IUserSubscriptionRepository userSubscriptionRepository)
        {
            _logger = logger;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
        }

        public async Task<UserSubscription> ActivateSubscriptionAsync(RevenueCatWebhookPayload payload)
        {

            var userId = int.Parse(payload.Event.AppUserId);
            var plan = await _subscriptionPlanRepository.GetByProductIdAsync(payload.Event.ProductId);

            if (plan == null)
            {
                _logger.LogError($"No subscription plan found for ProductId: {payload.Event.ProductId}");
                throw new Exception("Invalid subscription plan");
            }

            //Deactivate all current subscriptions for this user
            await _userSubscriptionRepository.DeactivateAllForUserAsync(userId);

            var newUserSubscription = new UserSubscription
            {
                UserId = userId,
                SubscriptionPlanId = plan.Id,
                StartDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.PurchasedAtMs).UtcDateTime,
                EndDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.ExpirationAtMs).UtcDateTime,
                Status = SubscriptionStatus.Active,
                TransactionId = payload.Event.TransactionId,
                Platform = payload.Event.Store,
                Provider = PaymentProvider.RevenueCat,
                Environment = payload.Event.Environment == "PRODUCTION" ? Environment.Production : Environment.Sandbox,
                IsActive = true
            };

            await _userSubscriptionRepository.AddAsync(newUserSubscription);
            _logger.LogInfo($"UserSubscription created for user {userId} with plan {plan.Name}");
            return newUserSubscription;
        }

        public async Task<UserSubscription> ExtendSubscriptionAsync(RevenueCatWebhookPayload payload)
        {
           
            var userId = int.Parse(payload.Event.AppUserId);

            var plan = await _subscriptionPlanRepository.GetByProductIdAsync(payload.Event.ProductId);

            if (plan == null)
            {
                _logger.LogError($"No subscription plan found for ProductId: {payload.Event.ProductId}");
                throw new Exception("Invalid subscription plan");
            }

            var userSubscription = await _userSubscriptionRepository.GetActiveSubscription(userId, plan.Id);
            if(userSubscription != null)
            {
                // Extend the subscription
                userSubscription.EndDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.ExpirationAtMs).UtcDateTime;
                userSubscription.Status = SubscriptionStatus.Active;
                userSubscription.IsActive = true;
                userSubscription.TransactionId = payload.Event.TransactionId;

               await _userSubscriptionRepository.Update(userSubscription);
               _logger.LogInfo($"UserSubscription extended for user {userId} on plan {plan.Id}");
            }

            return userSubscription;
        }

        public async Task<bool> DeactivateExpiredUserSubscriptionAsync(RevenueCatWebhookPayload payload)
        {
            var userId = int.Parse(payload.Event.AppUserId);

            // Look up the subscription plan by productId from the payload
            var plan = await _subscriptionPlanRepository.GetByProductIdAsync(payload.Event.ProductId);
            if (plan == null)
            {
                _logger.LogError($"No subscription plan found for ProductId: {payload.Event.ProductId}");
                return false;
            }

            // Get the user's currently active subscription on that plan
            var subscription = await _userSubscriptionRepository.GetActiveSubscription(userId, plan.Id);
            if (subscription == null)
            {
                _logger.LogWarn($"No active subscription found to deactivate for user {userId} on plan {plan.Id}");
                return false;
            }

            subscription.Status = SubscriptionStatus.Expired;
            subscription.IsActive = false;
            subscription.IsAutoRenew = false;

            await _userSubscriptionRepository.Update(subscription);

            _logger.LogInfo($"Subscription deactivated for user {userId} on plan {plan.Id}");
            return true;
        }

        public async Task<bool> ProcessCancellationAsync(RevenueCatWebhookPayload payload)
        {
            var userId = int.Parse(payload.Event.AppUserId);

            // Look up the subscription plan by productId from the payload
            var plan = await _subscriptionPlanRepository.GetByProductIdAsync(payload.Event.ProductId);
            if (plan == null)
            {
                _logger.LogError($"No subscription plan found for ProductId: {payload.Event.ProductId}");
                return false;
            }

            // Get the user's currently active subscription on that plan
            var subscription = await _userSubscriptionRepository.GetActiveSubscription(userId, plan.Id);
            if (subscription == null)
            {
                _logger.LogWarn($"No active subscription found to deactivate for user {userId} on plan {plan.Id}");
                return false;
            }

            subscription.IsAutoRenew = false; //cancel auto renewal

            await _userSubscriptionRepository.Update(subscription);

            _logger.LogInfo($"Subscription auto-renewal is cancelled for user {userId} on plan {plan.Id}");
            return true;
        }
    }
}