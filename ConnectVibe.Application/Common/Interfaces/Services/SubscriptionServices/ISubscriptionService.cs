using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities.Subscriptions;

namespace Fliq.Application.Common.Interfaces.Services.SubscriptionServices
{
    public interface ISubscriptionService
    {
        Task<UserSubscription> ActivateSubscriptionAsync(RevenueCatWebhookPayload payload);

        Task<UserSubscription> ExtendSubscriptionAsync(RevenueCatWebhookPayload payload);

        Task<bool> DeactivateExpiredUserSubscriptionAsync(RevenueCatWebhookPayload payload);

        Task<bool> ProcessCancellationAsync(RevenueCatWebhookPayload payload);

    }
}