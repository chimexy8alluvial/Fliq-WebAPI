namespace Fliq.Domain.Entities
{
    public class Payment : Record
    {
        public int UserId { get; set; }

        public PaymentProvider Provider { get; set; }

        public string TransactionId { get; set; } = default!;

        public string ProductId { get; set; } = default!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = default!;

        public DateTime PaymentDate { get; set; }

        public PaymentStatus Status { get; set; }

        public PaymentMethod Method { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public Environment Environment { get; set; }

        public DateTime DateCreated { get; set; }
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
        Unkmown,
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