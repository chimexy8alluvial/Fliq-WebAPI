using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event;
namespace Fliq.Application.Authentication.Common.Event
{
    public record CreateEventResult
    (
       int Id,
       EventType EventType,
       string eventTitle,
       string eventDescription,
       DateTime startDate,
       DateTime endDate,
       //TimeZoneInfo timeZone,
       Location Location,
       int capacity,
       string optional,
       int UserId,
       List<EventDocument> Docs,
       List<ProfilePhoto> Photos,
       string StartAge,
       string EndAge
    );

    public enum EventType
    {
        Physical,
        Live
    }
}
