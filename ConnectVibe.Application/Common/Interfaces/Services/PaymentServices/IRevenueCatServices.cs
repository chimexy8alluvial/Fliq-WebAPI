using ErrorOr;
using Fliq.Application.Payments.Common;

namespace Fliq.Application.Common.Interfaces.Services.PaymentServices
{
    public interface IRevenueCatServices
    {
        Task<bool> ProcessInitialPurchaseAsync(RevenueCatWebhookPayload payload);

        Task<bool> ProcessRenewalAsync(RevenueCatWebhookPayload payload);
        Task<ErrorOr<bool>> RefundTransactionAsync(string transactionId);
    }
}