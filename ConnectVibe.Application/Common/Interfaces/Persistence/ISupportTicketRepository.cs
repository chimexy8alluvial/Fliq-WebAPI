using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicket> GetTicketByIdAsync(int ticketId);

        Task<SupportTicket> GetTicketBySupportTicketIdAsync(string supportTicketId);

        Task<List<HelpMessage>> GetMessagesForTicketAsync(int ticketId);

        Task<SupportTicket> CreateTicketAsync(SupportTicket ticket);

        Task AddMessageToTicketAsync(int ticketId, HelpMessage message);

        Task UpdateTicketStatusAsync(int ticketId, HelpRequestStatus status);

        Task<List<SupportTicket>> GetPaginatedSupportTicketsAsync(PaginationRequest paginationRequest);

        Task<int> GetTotalSupportTicketsCountAsync(string requestStatus = null);
    }
}