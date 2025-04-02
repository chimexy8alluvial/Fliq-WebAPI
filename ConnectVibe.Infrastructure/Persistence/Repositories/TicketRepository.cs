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
            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public EventTicket? GetEventTicketById(int id)
        {
            var result = _dbContext.EventTickets.SingleOrDefault(p => p.Id == id);
            return result;
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

        public async Task<int> GetMondayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventMondayTicketCount",
                    new
                    {
                        EventId = eventId,
                        TicketType = ticketType // Nullable int, maps to INT NULL in SQL
                    },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetTuesdayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventTuesdayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetWednesdayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventWednesdayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetThursdayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventThursdayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetFridayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventFridayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetSaturdayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventSaturdayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> GetSundayTicketCountAsync(int eventId, TicketType? ticketType)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "GetEventSundayTicketCount",
                    new { EventId = eventId, TicketType = ticketType },
                    commandType: CommandType.StoredProcedure);
                return count;
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

        #endregion

    }
}