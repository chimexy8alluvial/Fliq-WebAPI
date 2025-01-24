namespace Fliq.Contracts.Event
{
    public record SponsoredEventDetailsDto
    (
        string BusinessName,
        string BusinessAddress,
        string BusinessType,
        string ContactInformation,
        int SponsoringPlan,
        int TargetAudienceType,
        int NumberOfInvitees,
        decimal Budget,
        int PreferredLevelOfInvolvement,
        DateTime StartDate,
        DateTime EndDate
    );
}