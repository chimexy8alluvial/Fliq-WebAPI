using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public TicketRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Ticket ticket)
        {
            if (ticket != null)
            {
                _dbContext.Add(ticket);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Ticket request)
        {
            request.DateModified = DateTime.Now;
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public Ticket? GetTicketById(int id)
        {
            var result = _dbContext.Tickets.SingleOrDefault(p => p.Id == id);
            return result;
        }

        public List<Currency> GetCurrenciees()
        {
            var result = _dbContext.Currencies.ToList();
            return result;
        }

        public void AddEventTicket(EventTicket eventTicket)
        {
            if (eventTicket != null)
            {
                _dbContext.Add(eventTicket);
            }
            _dbContext.SaveChanges();
        }

        public void UpdateEventTicket(EventTicket request)
        { 
            request.DateModified= DateTime.Now;
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }


        public void UpdateRange(IEnumerable<Ticket> tickets)
        {
            _dbContext.Tickets.UpdateRange(tickets);
            _dbContext.SaveChanges();
        }

        public void AddEventTickets(IEnumerable<EventTicket> eventTickets)
        {
            _dbContext.EventTickets.AddRange(eventTickets);
            _dbContext.SaveChanges();
        }

        public EventTicket? GetEventTicketById(int id)
        {
            var result = _dbContext.EventTickets.SingleOrDefault(p => p.Id == id);
            return result;
        }
        public List<Ticket> GetTicketsByIds(List<int> ids)
        {
            return _dbContext.Tickets.Where(t => ids.Contains(t.Id)).ToList();
        }

        public async Task<List<GetEventsTicketsResult>> GetAllEventsTicketsForDashBoardAsync(GetEventsTicketsListRequest request)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = FilterEventsTicketsDynamicParams(request);
                var results = await connection.QueryAsync<GetEventsTicketsResult>(
                    "GetAllEventsTicketsForDashBoard",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return results.ToList();
            }
        }

        public List<EventTicket> GetEventTicketsByIds(List<int> eventTicketIds)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                // Convert list of IDs to a comma-separated string
                string eventTicketIdsParam = string.Join(",", eventTicketIds);

                var eventTickets = connection.Query<EventTicket, Ticket, EventTicket>(
                    "[dbo].[GetEventTicketsByIds]",
                    (eventTicket, ticket) =>
                    {
                        // Map EventTicket properties (including Record base class)
                        eventTicket.Ticket = ticket; // Assign the related Ticket
                        return eventTicket;
                    },
                    new { EventTicketIds = eventTicketIdsParam },
                    commandType: CommandType.StoredProcedure,
                    splitOn: "Ticket_Id" // Split on Ticket's Id column
                ).ToList();

                return eventTickets;
            }
        }

        public List<Ticket> GetTicketsByEventId(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                return connection.Query<Ticket>(
                    "[dbo].[GetTicketsByEventId]",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        private static DynamicParameters FilterEventsTicketsDynamicParams(GetEventsTicketsListRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@PageNumber", request.PaginationRequest.PageNumber);
            parameters.Add("@PageSize", request.PaginationRequest.PageSize);
            parameters.Add("@Category", request.Category);
            parameters.Add("@StatusFilter", request.StatusFilter);
            parameters.Add("@StartDate", request.StartDate);
            parameters.Add("@EndDate", request.EndDate);
            parameters.Add("@Location", request.Location);

            return parameters;
        }

        #region Count Queries
        public async Task<int> GetRegularTicketCountAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventRegularTicketCount",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure);

                return count;

            }            
        }

        public async Task<int> GetVipTicketCountAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventVipTicketCount",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure);

                return count;
            }
        }

        public async Task<int> GetVVipTicketCountAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventVVipTicketCount",
                    new { EventId = eventId }, 
                    commandType: CommandType.StoredProcedure);

                return count;
            }
        }

        public async Task<int> GetOtherTicketCountAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventOtherTicketCount",
                    new { EventId = eventId }, 
                    commandType: CommandType.StoredProcedure);

                return count;
            }
        }

        public async Task<int> GetTotalTicketCountAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventTotalTicketCount",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure);

                return count;
            }
        }

        public async Task<Dictionary<DayOfWeek, int>> GetWeeklyTicketCountAsync(
        int eventId,
        DateTime? startDate,
        DateTime? endDate,
        TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("EventId", eventId);
                parameters.Add("StartDate", startDate, dbType: DbType.Date);
                parameters.Add("EndDate", endDate, dbType: DbType.Date);
                parameters.Add("TicketType", (int?)ticketType);

                var results = await connection.QueryAsync<(int DayOfWeek, int TicketCount)>(
                    "GetEventWeeklyTicketCount",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return results.ToDictionary(
                    r => (DayOfWeek)r.DayOfWeek,
                    r => r.TicketCount);
            }
        }



        public async Task<decimal> GetEventTicketGrossRevenueAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var revenue = await connection.ExecuteScalarAsync<decimal>(
                    "GetEventTicketGrossRevenue",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure);
                return revenue;
            }
        }

        public async Task<decimal> GetEventTicketNetRevenueAsync(int eventId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var netRevenue = await connection.ExecuteScalarAsync<decimal>(
                    "GetEventTicketNetRevenue",
                    new { EventId = eventId },
                    commandType: CommandType.StoredProcedure);
                return netRevenue;
            }
        }
        #endregion

    }
}