namespace Fliq.Domain.Entities
{
    public class Subscription : Record
    {
        public int UserId { get; set; }

        public string ProductId { get; set; } = default!;

        public DateTime StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool IsActive { get; set; }

        public PaymentProvider Provider { get; set; }

        public SubscriptionStatus Status { get; set; }

        public Environment Environment { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public enum SubscriptionStatus
    {
        Active,
        Expired,
        Cancelled
    }
}