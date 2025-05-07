namespace Fliq.Application.DashBoard.Common
{
    public record GetEventsTicketsResult(
         string EventTitle,
         string CreatedBy,
         string EventStatus,
         string NatureOfEvent,
         int NumOfSoldTickets,
         DateTime CreatedOn);

}
