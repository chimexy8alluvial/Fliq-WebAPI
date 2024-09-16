using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event;
namespace Fliq.Application.Event.Common
{
    public record CreateEventResult
    (
      int Id,
     EventType EventType,
    string EventTitle,
    string EventDescription,
    DateTime StartDate,
    DateTime EndDate,
    Location Location,
    int Capacity,
    List<EventMedia> Media,
    string StartAge,
    string EndAge,
    string EventCategory,
    bool SponsoredEvent,
    SponsoredEventDetail SponsoredEventDetail,
    EventCriteria EventCriteria,
    TicketType TicketType,
    int UserId,
    UserProfile User
    );

    public enum EventType
    {
        Physical,
        Live
    }
}
