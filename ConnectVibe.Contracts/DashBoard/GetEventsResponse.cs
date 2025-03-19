namespace Fliq.Contracts.DashBoard
{
    public record GetEventsResponse
    (
       string EventTitle,
        string CreatedBy,
        string Status,
        int Attendees,
        string EventCategory,
        DateTime CreatedOn
    );
}
