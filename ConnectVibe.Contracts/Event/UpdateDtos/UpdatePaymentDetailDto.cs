namespace Fliq.Contracts.Event.UpdateDtos
{
    public class UpdatePaymentDetailDto
    {
        public int Id { get; set; }
        public string? Account { get; set; }
        public string? Bank { get; set; }
        public string? SortCode { get; set; }
        public string? AccountNumber { get; set; }
    }
}