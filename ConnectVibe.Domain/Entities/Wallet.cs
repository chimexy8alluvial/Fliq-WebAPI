namespace Fliq.Domain.Entities
{
    public class Wallet : Record
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal Balance { get; set; }
    }
}