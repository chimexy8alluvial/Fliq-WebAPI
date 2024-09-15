using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        int Id,
        EventTypeDto EventType,
        string eventTitle,
        string eventDescription,
        DateTime startDate,
        DateTime endDate,
        LocationDto Location,
        int capacity,
        int UserId,
        List<EventMediaDto> Docs,
        string StartAge,
        string EndAge,
        string EventCategory,
        bool SponsoredEvent,
        SponsoredEventDetailsDto SponsoredEventDetail,
        EventCriteriaDto EventCriteria,
        TicketTypeDto TicketType
    );
}
