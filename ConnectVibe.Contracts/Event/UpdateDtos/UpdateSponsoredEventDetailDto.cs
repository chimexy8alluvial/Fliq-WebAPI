namespace Fliq.Contracts.Event.UpdateDtos
{
    public record UpdateSponsoredEventDetailDto
    {
        public int Id { get; set; }
        public string? BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public string? BusinessType { get; set; }
        public string? ContactInfromation { get; set; }
        public int? SponsoringPlan { get; set; }
        public int? TargetAudienceType { get; set; }

        public decimal? Budget { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? PreferedLevelOfInvolvement { get; set; }
    }
}