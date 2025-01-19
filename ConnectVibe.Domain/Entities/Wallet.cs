namespace Fliq.Domain.Entities
{
    public class Wallet : Record
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}