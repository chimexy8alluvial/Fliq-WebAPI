namespace Fliq.Contracts.Event
{
    public record SponsoredEventDetailsDto
    (
        string BusinessName,
        string BusinessAddress,
        string BusinessType,
        string ContactInfromation,
        int SponsoringPlan,
        int TargetAudienceType,
        int NumberOfInvitees,
        decimal Budget,
        int PreferedLevelOfInvolvement,
        DateTime StartDate,
        DateTime EndDate
    );
}