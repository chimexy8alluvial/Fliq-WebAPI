using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventDetailsRequest(
        int Id,
        string Email,
        string eventTitle,
        string eventDescription,
        DateTime startDate,
        DateTime endDate,
        TimeZoneInfo timeZone,
        LocationDto Location,
        int capacity,
        string optional
    );
     
}
