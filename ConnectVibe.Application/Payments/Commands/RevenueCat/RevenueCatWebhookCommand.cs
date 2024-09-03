using ErrorOr;
using Fliq.Application.Payments.Common;
using MediatR;

namespace Fliq.Application.Payments.Commands.RevenueCat
{
    public class RevenueCatWebhookCommand : IRequest<ErrorOr<RevenueCatWebhookResult>>
    {
        public EventInfo Event { get; set; } = default!;
        public string ApiVersion { get; set; } = default!;

        public class EventInfo
        {
            public long EventTimestampMs { get; set; } = default!;
            public string ProductId { get; set; } = default!;
            public string PeriodType { get; set; } = default!;
            public long PurchasedAtMs { get; set; } = default!;
            public long ExpirationAtMs { get; set; } = default!;
            public string Environment { get; set; } = default!;
            public string EntitlementId { get; set; } = default!;
            public List<string> EntitlementIds { get; set; } = default!;
            public string PresentedOfferingId { get; set; } = default!;
            public string TransactionId { get; set; } = default!;
            public string OriginalTransactionId { get; set; } = default!;
            public bool IsFamilyShare { get; set; } = default!;
            public string CountryCode { get; set; } = default!;
            public string AppUserId { get; set; } = default!;
            public List<string> Aliases { get; set; } = default!;
            public string OriginalAppUserId { get; set; } = default!;
            public string Currency { get; set; } = default!;
            public decimal Price { get; set; } = default!;
            public decimal PriceInPurchasedCurrency { get; set; } = default!;
            public Dictionary<string, SubscriberAttribute> SubscriberAttributes { get; set; } = default!;
            public string Store { get; set; } = default!;
            public double TakehomePercentage { get; set; } = default!;
            public string OfferCode { get; set; } = default!;
            public string Type { get; set; } = default!;
            public string Id { get; set; } = default!;
            public string AppId { get; set; } = default!;
        }

        public class SubscriberAttribute
        {
            public long UpdatedAtMs { get; set; } = default!;
            public string Value { get; set; } = default!;
        }
    }
}