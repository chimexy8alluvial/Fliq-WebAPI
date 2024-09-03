namespace Fliq.Contracts.Payments
{
    public record PaymentResponse(
    bool Success,
    string? Message
);
}