using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IUserRepository _userRepository;

        public EventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _userRepository = userRepository;
        }

        public void Add(Events createEvent)
        {
            if (createEvent != null)
            {
                _dbContext.Add(createEvent);
            }
            _dbContext.SaveChanges();
        }

        public void Update(Events request)
        {
            request.DateModified = DateTime.Now;

            _dbContext.Update(request);
            _dbContext.SaveChanges();
        }

        public Events? GetEventById(int id)
        {
            var result = _dbContext.Events.SingleOrDefault(p => p.Id == id);
            return result;
        }

        public List<Events> GetAllEvents()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                //Highlight this
                const string SQL = "SELECT * FROM Events WHERE EventTitle IS NOT NULL";

                // Ensure 'Events' is a class that matches the schema of your 'Events' table
                var results = connection.Query<Events>(SQL);

                return results.ToList();
            }
        }
        public async Task<IEnumerable<EventWithUsername>> GetAllEventsForDashBoardAsync(GetEventsListRequest query)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterListDynamicParams(query);

            var results = await connection.QueryAsync<dynamic>(
                "sp_GetAllEventsForDashBoard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var eventWithUsernames = results.Select(r => new EventWithUsername
            {
                Event = new Events
                {
                    EventTitle = r.EventTitle,
                    UserId = r.UserId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    SponsoredEvent = (bool)r.SponsoredEvent,
                    DateCreated = r.DateCreated,
                    Tickets = new List<Ticket> { new Ticket { Id = r.TicketCount } }
                },
                Username = string.Concat(r.FirstName ?? "", " ", r.LastName ?? "")
            }).ToList();

            return eventWithUsernames;
        }
        public async Task<IEnumerable<EventWithUsername>> GetAllCancelledEventsForDashBoardAsync(GetEventsListRequest query)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterListDynamicParams(query);

            var results = await connection.QueryAsync<dynamic>(
                "sp_GetAllCancelledEventsForDashBoard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var eventWithUsernames = results.Select(r => new EventWithUsername
            {
                Event = new Events
                {
                    EventTitle = r.EventTitle,
                    UserId = r.UserId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    SponsoredEvent = (bool)r.SponsoredEvent,
                    DateCreated = r.DateCreated,
                    Tickets = new List<Ticket> { new Ticket { Id = r.TicketCount } }
                },
                Username = string.Concat(r.FirstName ?? "", " ", r.LastName ?? "")
            }).ToList();

            return eventWithUsernames;
        }
        public async Task<IEnumerable<EventWithUsername>> GetAllFlaggedEventsForDashBoardAsync(GetEventsListRequest query)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterListDynamicParams(query);

            var results = await connection.QueryAsync<dynamic>(
                "sp_GetAllFlaggedEventsForDashBoard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var eventWithUsernames = results.Select(r => new EventWithUsername
            {
                Event = new Events
                {
                    EventTitle = r.EventTitle,
                    UserId = r.UserId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    SponsoredEvent = (bool)r.SponsoredEvent,
                    DateCreated = r.DateCreated,
                    Tickets = new List<Ticket> { new Ticket { Id = r.TicketCount } }
                },
                Username = string.Concat(r.FirstName ?? "", " ", r.LastName ?? "")
            }).ToList();

            return eventWithUsernames;
        }

        private static DynamicParameters FilterListDynamicParams(GetEventsListRequest query)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pageNumber", query.PaginationRequest!.PageNumber);
            parameters.Add("@pageSize", query.PaginationRequest.PageSize);
            parameters.Add("@category", query.Category);
            parameters.Add("@startDate", query.StartDate);
            parameters.Add("@endDate", query.EndDate);
            parameters.Add("@location", query.Location);
            return parameters;
        }

        #region Count Queries

        public async Task<int> CountAllEvents()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllEvents", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        public async Task<int> CountAllSponsoredEvents()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllSponsoredEvents", commandType: CommandType.StoredProcedure);
                return count;
            }
        }

        #endregion
    }
}