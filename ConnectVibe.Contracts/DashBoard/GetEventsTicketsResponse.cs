namespace Fliq.Contracts.DashBoard
{
    public record GetEventsTicketsResponse(
      string EventTitle,
      string CreatedBy,
      string EventStatus,
      string NatureOfEvent,
      int NumOfSoldTickets,
      DateTime CreatedOn);
}
