

namespace Fliq.Domain.Entities.Subscriptions
{
    public class SubscriptionPlan : Record
    {
        public string Name { get; set; } = string.Empty; // "Gold"
        public string ProductId { get; set; } = string.Empty; // RevenueCat product ID
        public string? Description { get; set; }
        public ICollection<SubscriptionPlanPrice>? Prices { get; set; }
    }
}
