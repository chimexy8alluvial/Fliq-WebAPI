namespace Fliq.Contracts.Event
{
    public record EventPaymentDetailDto(string Account, string Bank, string SortCode, string AccountNumber);
}