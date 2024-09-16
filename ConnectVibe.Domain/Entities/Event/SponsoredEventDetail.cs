namespace Fliq.Domain.Entities.Event
{
    public class SponsoredEventDetail
    {
        public int Id { get; set; }
        public string BusinessName { get; set; } = default!;
        public string BusinessAddress { get; set; } = default!;
        public List<string> BusinessType { get; set; } = default!;
        public string ContactInfromation { get; set; } = default!;
        public SponsoringBudget SponsoringBudget { get; set; } = default!;
        public List<string> TargetAudienceType { get; set; } = default!;
        public int NumberOfInvitees { get; set; } = default!;
        public float Budget { get; set; } = default!;
        public string DurationOfSponsorship { get; set; } = default!;
        public List<string> PreferedLevelOfInvolvement { get; set; } = default!;
    }

    public enum SponsoringBudget
    {
        Gold,
        Silver,
        Bronze
    }
}
