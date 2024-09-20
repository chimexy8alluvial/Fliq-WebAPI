namespace Fliq.Contracts.Event
{
    public record SponsoredEventDetailsDto
    (
        string BusinessName,
        string BusinessAddress,
        string BusinessType,
        string ContactInfromation,
        SponsoringBudgetDto SponsoringBudget,
        string TargetAudienceType,
        int NumberOfInvitees,
        double Budget,
        string DurationOfSponsorship,
        string PreferedLevelOfInvolvement
    );
}
