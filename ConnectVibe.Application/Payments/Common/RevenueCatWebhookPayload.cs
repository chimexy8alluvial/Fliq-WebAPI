using System.Text.Json.Serialization;

namespace Fliq.Application.Payments.Common
{
    public class RevenueCatWebhookPayload
    {
        [JsonPropertyName("event")]
        public EventInfo Event { get; set; } = default!;

        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = default!;

        public class EventInfo
        {
            [JsonPropertyName("event_timestamp_ms")]
            public long EventTimestampMs { get; set; } = default!;

            [JsonPropertyName("product_id")]
            public string ProductId { get; set; } = default!;

            [JsonPropertyName("period_type")]
            public string PeriodType { get; set; } = default!;

            [JsonPropertyName("purchased_at_ms")]
            public long PurchasedAtMs { get; set; } = default!;

            [JsonPropertyName("expiration_at_ms")]
            public long ExpirationAtMs { get; set; } = default!;

            [JsonPropertyName("environment")]
            public string Environment { get; set; } = default!;

            [JsonPropertyName("entitlement_id")]
            public string? EntitlementId { get; set; } = default!;

            [JsonPropertyName("entitlement_ids")]
            public List<string> EntitlementIds { get; set; } = default!;

            [JsonPropertyName("presented_offering_id")]
            public string? PresentedOfferingId { get; set; } = default!;

            [JsonPropertyName("transaction_id")]
            public string TransactionId { get; set; } = default!;

            [JsonPropertyName("original_transaction_id")]
            public string OriginalTransactionId { get; set; } = default!;

            [JsonPropertyName("is_family_share")]
            public bool IsFamilyShare { get; set; } = default!;

            [JsonPropertyName("country_code")]
            public string CountryCode { get; set; } = default!;

            [JsonPropertyName("app_user_id")]
            public string AppUserId { get; set; } = default!;

            [JsonPropertyName("aliases")]
            public List<string> Aliases { get; set; } = default!;

            [JsonPropertyName("original_app_user_id")]
            public string OriginalAppUserId { get; set; } = default!;

            [JsonPropertyName("currency")]
            public string Currency { get; set; } = default!;

            [JsonPropertyName("price")]
            public decimal Price { get; set; } = default!;

            [JsonPropertyName("price_in_purchased_currency")]
            public decimal PriceInPurchasedCurrency { get; set; } = default!;

            [JsonPropertyName("subscriber_attributes")]
            public Dictionary<string, SubscriberAttribute> SubscriberAttributes { get; set; } = default!;

            [JsonPropertyName("store")]
            public string Store { get; set; } = default!;

            [JsonPropertyName("takehome_percentage")]
            public double TakehomePercentage { get; set; } = default!;

            [JsonPropertyName("offer_code")]
            public string? OfferCode { get; set; } = default!;

            [JsonPropertyName("type")]
            public string Type { get; set; } = default!;

            [JsonPropertyName("id")]
            public string Id { get; set; } = default!;

            [JsonPropertyName("app_id")]
            public string AppId { get; set; } = default!;
        }

        public class SubscriberAttribute
        {
            [JsonPropertyName("updated_at_ms")]
            public long UpdatedAtMs { get; set; } = default!;

            [JsonPropertyName("value")]
            public string Value { get; set; } = default!;
        }
    }
}