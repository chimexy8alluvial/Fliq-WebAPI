

namespace Fliq.Domain.Entities.Subscriptions
{
    public class SubscriptionPlanPrice : Record
    {
        public int SubscriptionPlanId { get; set; } 
        public SubscriptionPlan SubscriptionPlan { get; set; } = default!;

        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string? Store { get; set; } // "ios", "android", "web"
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    }
}
