using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class SponsoredEventDetail : Record
    {
        public string BusinessName { get; set; } = default!;
        public string BusinessAddress { get; set; } = default!;
        public string BusinessType { get; set; } = default!;
        public string ContactInformation { get; set; } = default!;
        public SponsoringPlan SponsoringPlan { get; set; } = default!;
        public TargetAudienceType TargetAudienceType { get; set; } = default!;

        public decimal Budget { get; set; } = default!;

        public DateTime StartDate { get; set; } = default!;
        public DateTime EndDate { get; set; }
        public LevelOfInvolvement PreferedLevelOfInvolvement { get; set; } = default!;
    }
}