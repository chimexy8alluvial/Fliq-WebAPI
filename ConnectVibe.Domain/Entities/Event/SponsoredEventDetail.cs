using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class SponsoredEventDetail
    {
        public int Id { get; set; }
        public string BusinessName { get; set; } = default!;
        public string BusinessAddress { get; set; } = default!;
        public string BusinessType { get; set; } = default!;
        public string ContactInfromation { get; set; } = default!;
        public SponsoringBudget SponsoringBudget { get; set; } = default!;
        public string TargetAudienceType { get; set; } = default!;
        public int NumberOfInvitees { get; set; } = default!;
        public double Budget { get; set; } = default!;
        public string DurationOfSponsorship { get; set; } = default!;
        public string PreferedLevelOfInvolvement { get; set; } = default!;
    }

    
}
