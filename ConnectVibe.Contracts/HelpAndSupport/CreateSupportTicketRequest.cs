namespace Fliq.Contracts.HelpAndSupport
{
    public record CreateSupportTicketRequest(
    string Title,
    string RequesterName,
    int RequestType);

    public record AddMessageRequest(
        string Content);

    public record UpdateStatusRequest(
        int NewStatus);

    public record GetPaginatedTicketsRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string RequestStatus = null);
}