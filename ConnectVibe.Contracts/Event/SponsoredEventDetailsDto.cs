namespace Fliq.Contracts.Event
{
    public record SponsoredEventDetailsDto
    (
        string BusinessName,
        string BusinessAddress,
        List<string> BusinessType,
        string ContactInfromation,
        SponsoringBudgetDto SponsoringBudget,
        List<string> TargetAudienceType,
        int NumberOfInvitees,
        float Budget,
        string DurationOfSponsorship,
        List<string> PreferedLevelOfInvolvement
    );
}
