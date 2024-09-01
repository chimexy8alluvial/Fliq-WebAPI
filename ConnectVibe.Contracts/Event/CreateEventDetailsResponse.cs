using ConnectVibe.Contracts.Profile;
namespace Fliq.Contracts.Event
{
    public class CreateEventDetailsResponse
    (
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
