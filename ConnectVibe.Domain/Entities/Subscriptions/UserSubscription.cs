

namespace Fliq.Domain.Entities.Subscriptions
{
    public class UserSubscription : Record
    {
        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = default!;

        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsAutoRenew { get; set; } = true;

        public string Platform { get; set; } = string.Empty; // "ios", "android", "web"
        public string TransactionId { get; set; } = string.Empty;
        public PaymentProvider Provider { get; set; }
        public SubscriptionStatus Status { get; set; }
        public Environment Environment { get; set; }
    }
}
