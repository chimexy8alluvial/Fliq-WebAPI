using Fliq.Contracts.Profile.UpdateDtos;

namespace Fliq.Contracts.Event.UpdateDtos
{
    public record GetEventResponse
    {
        public int Id { get; set; }
        public int? EventType { get; set; }
        public string? EventTitle { get; set; } = default!;
        public string? EventDescription { get; set; } = default!;
        public int? EventCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public UpdateLocationDto? Location { get; set; } = default!;
        public int? Capacity { get; set; }
        public List<UpdateEventMediaDto> Media { get; set; } = default!;
        public int MinAge { get; set; }
        public int Maxge { get; set; }
        public bool SponsoredEvent { get; set; } = default!;
        public UpdateSponsoredEventDetailDto? SponsoredEventDetail { get; set; } = default!;
        public UpdateEventCriteriaDto EventCriteria { get; set; } = default!;
        public List<UpdateTicketDto>? Tickets { get; set; } = default!;
        public int UserId { get; set; } = default!;
        public UpdatePaymentDetailDto? EventPaymentDetail { get; set; } = default!;
        public bool InviteesException { get; set; } = default!;
    }
}