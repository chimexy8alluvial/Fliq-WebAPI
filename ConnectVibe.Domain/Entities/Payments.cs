namespace Fliq.Domain.Entities
{
    public class Payment : Record
    {
        public int UserId { get; set; }

        public PaymentProvider Provider { get; set; }

        public string TransactionId { get; set; } = default!;

        public string? ProductId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = default!;

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public PaymentMethod Method { get; set; }

        public int? SubscriptionPlanId { get; set; }
        public int? UserSubscriptionId { get; set; }

    }

    public enum PaymentProvider
    {
        RevenueCat,
        Stripe,
        PayPal
    }

    public enum PaymentStatus
    {
        Success,
        Failed,
        Pending,
        Refunded
    }

    public enum PaymentMethod
    {
        Unknown,
        CreditCard,
        PayPal,
        ApplePay,
        GooglePay,
        BankTransfer
    }

    public enum Environment
    {
        Production,
        Sandbox
    }
}