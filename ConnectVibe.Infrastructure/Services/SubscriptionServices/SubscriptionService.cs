using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities;
using Environment = Fliq.Domain.Entities.Environment;

namespace Fliq.Infrastructure.Services.SubscriptionServices
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILoggerManager _logger;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository, ILoggerManager logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        public async Task<Subscription> ActivateSubscriptionAsync(RevenueCatWebhookPayload payload)
        {
            await Task.CompletedTask;
            var subscription = new Subscription
            {
                UserId = int.Parse(payload.Event.AppUserId),
                ProductId = payload.Event.ProductId,
                StartDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.PurchasedAtMs).UtcDateTime,
                ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.ExpirationAtMs).UtcDateTime,
                Status = SubscriptionStatus.Active,
                Provider = PaymentProvider.RevenueCat,
                Environment = payload.Event.Environment == "PRODUCTION" ? Environment.Production : Environment.Sandbox
            };

            _subscriptionRepository.Add(subscription);
            _logger.LogInfo($"Subscription created for user {payload.Event.AppUserId} and product {payload.Event.ProductId}");
            return subscription;
        }

        public async Task<Subscription> ExtendSubscriptionAsync(RevenueCatWebhookPayload payload)
        {
            await Task.CompletedTask;
            var subscription = _subscriptionRepository.GetSubscriptionByUserIdAndProductIdAsync(int.Parse(payload.Event.AppUserId), payload.Event.ProductId);

            if (subscription != null)
            {
                subscription.ExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(payload.Event.ExpirationAtMs).UtcDateTime;
                subscription.Status = SubscriptionStatus.Active;

                _subscriptionRepository.Update(subscription);
                _logger.LogInfo($"Subscription extended for user {payload.Event.AppUserId} and product {payload.Event.ProductId}");
            }
            return subscription;
        }
    }
}