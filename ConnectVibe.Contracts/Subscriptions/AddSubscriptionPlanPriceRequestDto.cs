namespace Fliq.Contracts.Subscriptions
{
    public record AddSubscriptionPlanPriceRequestDto(int SubscriptionPlanId,
        decimal Amount,
        string Currency,
        string Country,
        DateTime EffectiveFrom,
        string? Store);
}
