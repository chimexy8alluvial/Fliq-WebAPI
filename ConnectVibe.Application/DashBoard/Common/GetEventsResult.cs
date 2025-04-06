namespace Fliq.Application.DashBoard.Common
{
    public record GetEventsResult
    (
       string EventTitle,
        string CreatedBy,
        string Status,
        int Attendees,
        string Type,
        DateTime CreatedOn
    );

   
}