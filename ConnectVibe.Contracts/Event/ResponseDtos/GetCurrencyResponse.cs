namespace Fliq.Contracts.Event.ResponseDtos
{
    public class GetCurrencyResponse
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime? DateModified { get; set; }
        public bool IsDeleted { get; set; }
        public string CurrencyCode { get; set; } = default!;
    }
}