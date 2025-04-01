namespace Fliq.Contracts.DashBoard
{
    public record GetEventsTicketsResponse(
      string EventTitle,
      string CreatedBy,
      string EventStatus,
      string Type,
      int NumOfSoldTickets,
      DateTime CreatedOn);
}
