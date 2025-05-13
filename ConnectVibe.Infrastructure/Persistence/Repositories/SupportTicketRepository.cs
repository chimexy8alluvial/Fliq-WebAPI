using Dapper;
using Fliq.Application.Common.Helpers;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Enums;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly ILoggerManager _loggerManager;
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public SupportTicketRepository(ILoggerManager loggerManager, FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _loggerManager = loggerManager;
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public async Task<SupportTicket> GetTicketByIdAsync(int ticketId)
        {
            var ticket = _dbContext.SupportTickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null)
            {
                _loggerManager.LogWarn($"Ticket with ID {ticketId} not found.");
            }
            return ticket;
        }

        public async Task<SupportTicket> GetTicketBySupportTicketIdAsync(string supportTicketId)
        {
            var ticket = _dbContext.SupportTickets.FirstOrDefault(t => t.TicketId == supportTicketId);
            if (ticket == null)
            {
                _loggerManager.LogWarn($"Ticket with SupportTicketId {supportTicketId} not found.");
            }
            return ticket;
        }

        public async Task<List<HelpMessage>> GetMessagesForTicketAsync(int ticketId)
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            return ticket?.Messages ?? new List<HelpMessage>();
        }

        public async Task<SupportTicket> CreateTicketAsync(SupportTicket ticket)
        {
            ticket.TicketId = IdGeneratorHelper.GenerateSupportTicketId();
            _dbContext.SupportTickets.Add(ticket);
            _dbContext.SaveChanges();
            _loggerManager.LogInfo($"Ticket created with ID {ticket.Id}.");
            return ticket;
        }

        public async Task AddMessageToTicketAsync(int ticketId, HelpMessage message)
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            if (ticket != null)
            {
                message.SupportTicketId = ticketId;
                ticket.Messages.Add(message);
                _dbContext.SupportTickets.Update(ticket);
                _loggerManager.LogInfo($"Message added to ticket {ticketId}.");
            }
        }

        public async Task UpdateTicketStatusAsync(int ticketId, HelpRequestStatus status)
        {
            var ticket = await GetTicketByIdAsync(ticketId);
            if (ticket != null)
            {
                ticket.RequestStatus = status;
                _dbContext.SupportTickets.Update(ticket);
                _dbContext.SaveChanges();
                _loggerManager.LogInfo($"Ticket {ticketId} status updated to {status}.");
            }
        }

        public async Task<List<SupportTicket>> GetPaginatedSupportTicketsAsync(PaginationRequest paginationRequest, int? requestType = null, int? requestStatus = null)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    _loggerManager.LogInfo($"Fetching paginated support tickets. Page: {paginationRequest.PageNumber}, PageSize: {paginationRequest.PageNumber}, RequestType: {requestType}, RequestStatus: {requestStatus}");

                    var parameters = new DynamicParameters();
                    parameters.Add("@PageNumber", paginationRequest.PageNumber);
                    parameters.Add("@PageSize", paginationRequest.PageSize);
                    parameters.Add("@RequestType", requestType);
                    parameters.Add("@RequestStatus", requestStatus);

                    var tickets = await connection.QueryAsync<SupportTicket>(
                        "GetPaginatedSupportTickets",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return tickets.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching paginated support tickets: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetTotalSupportTicketsCountAsync(int? requestType = null, int? requestStatus = null)
        {
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    _loggerManager.LogInfo(
                    $"Fetching total support tickets count. RequestType: {requestType}, RequestStatus: {requestStatus}");

                    var parameters = new DynamicParameters();
                    parameters.Add("@RequestType", requestType);
                    parameters.Add("@RequestStatus", requestStatus);

                    var sql = @"SELECT COUNT(*) FROM SupportTickets 
                    WHERE 
                        IsDeleted = 0
                        AND (@RequestType IS NULL OR RequestType = @RequestType)
                        AND (@RequestStatus IS NULL OR RequestStatus = @RequestStatus)";

                    var totalCount = await connection.ExecuteScalarAsync<int>(sql, parameters);
                    return totalCount;
                }
            }
            catch (Exception ex)
            {
                _loggerManager.LogError($"Error fetching total count of support tickets: {ex.Message}");
                throw;
            }
        }

        private static DynamicParameters CreateDynamicParameters(PaginationRequest paginationRequest)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pageNumber", paginationRequest.PageNumber);
            parameters.Add("@pageSize", paginationRequest.PageSize);

            return parameters;
        }
    }
}