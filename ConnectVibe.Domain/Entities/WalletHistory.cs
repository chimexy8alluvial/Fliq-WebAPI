namespace Fliq.Domain.Entities
{
    public class WalletHistory : Record
    {
        public int? Id { get; set; }
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; } = default!;
        public decimal Amount { get; set; }
        public WalletActivityType ActivityType { get; set; }
        public WalletTransactionStatus TransactionStatus { get; set; }
        public string? Description { get; set; }
        public string? FailureReason { get; set; }
    }

    public enum WalletActivityType
    {
        Deposit,
        Withdrawal,
        Refund,
    }

    public enum WalletTransactionStatus
    {
        Success,
        Failed,
        Pending
    }
}