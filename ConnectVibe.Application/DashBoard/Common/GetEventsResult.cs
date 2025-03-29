using Fliq.Domain.Entities.Event;

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

    public class EventWithUsername
    {


        public Events? Event { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}