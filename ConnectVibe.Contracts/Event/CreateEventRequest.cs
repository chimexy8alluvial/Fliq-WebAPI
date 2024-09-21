using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        int Id,
        EventTypeDto EventType,
        string EventTitle,
        string EventDescription,
        DateTime StartDate,
        DateTime EndDate,
        LocationDto Location,
        int Capacity,
        int UserId,
        List<EventMediaDto> Docs,
        string StartAge,
        string EndAge,
        string EventCategory,
        bool SponsoredEvent,
        SponsoredEventDetailsDto SponsoredEventDetail,
        EventCriteriaDto EventCriteria,
        List<TicketTypeDto> TicketType
    );
}
