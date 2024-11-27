using Fliq.Contracts.Profile.UpdateDtos;

namespace Fliq.Contracts.Event.UpdateDtos
{
    public record UpdateEventDto
    {
        public int Id { get; set; }
        public int? EventType { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public int? EventCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public UpdateLocationDto? Location { get; set; }
        public int? Capacity { get; set; }
        public List<UpdateEventMediaDto>? Media { get; set; }
        public int? MinAge { get; set; }
        public int? Maxge { get; set; }
        public bool? SponsoredEvent { get; set; }
        public UpdateSponsoredEventDetailDto? SponsoredEventDetail { get; set; }
        public UpdateEventCriteriaDto? EventCriteria { get; set; }
        public List<UpdateTicketDto>? Tickets { get; set; }
        public int UserId { get; set; }
        public UpdatePaymentDetailDto? EventPaymentDetail { get; set; }
        public bool? InviteesException { get; set; }
    }
}