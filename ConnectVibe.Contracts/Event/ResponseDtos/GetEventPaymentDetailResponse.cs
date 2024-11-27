namespace Fliq.Contracts.Event.ResponseDtos
{
    public class GetEventPaymentDetailResponse
    {
        public string Account { get; set; } = default!;
        public string Bank { get; set; } = default!;
        public string SortCode { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
    }
}