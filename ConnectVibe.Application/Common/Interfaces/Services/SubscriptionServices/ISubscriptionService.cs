using Fliq.Application.Payments.Common;
using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Services.SubscriptionServices
{
    public interface ISubscriptionService
    {
        Task<Subscription> ActivateSubscriptionAsync(RevenueCatWebhookPayload payload);

        Task<Subscription> ExtendSubscriptionAsync(RevenueCatWebhookPayload payload);
    }
}