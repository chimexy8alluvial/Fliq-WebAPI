namespace Fliq.Contracts.Subscriptions
{
    public record CreateSubscriptionPlanRequestDto(string Name, string ProductId, string? Description);
}
