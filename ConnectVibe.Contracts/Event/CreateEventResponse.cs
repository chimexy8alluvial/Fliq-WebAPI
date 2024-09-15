using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventResponse
    (
       int Id,
       EventTypeDto EventType,
       string eventTitle,
       string eventDescription,
       DateTime startDate,
       DateTime endDate,
       TimeZoneInfo timeZone,
       LocationDto Location,
       int capacity,
       string optional,
       int UserId,
       List<EventMediaDto> Docs
    );
}
