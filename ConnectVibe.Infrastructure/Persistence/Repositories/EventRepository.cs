using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.Explore;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using Newtonsoft.Json;
using System.Data;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly FliqDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;


        public EventRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory, IUserRepository userRepository, ILoggerManager logger)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _userRepository = userRepository;
            _logger = logger;
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
        public async Task<IEnumerable<GetEventsResult>> GetAllEventsForDashBoardAsync(GetEventsListRequest query)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterListDynamicParams(query);

            var results = await connection.QueryAsync<GetEventsResult>(
                "sp_GetAllEventsForDashBoard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return results.ToList();
        }
       
        public async Task<IEnumerable<GetEventsResult>> GetAllFlaggedEventsForDashBoardAsync(GetEventsListRequest query)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterListDynamicParams(query);

            var results = await connection.QueryAsync<GetEventsResult>(
                "sp_GetAllFlaggedEventsForDashBoard",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return results.ToList();
        }

        private static DynamicParameters FilterListDynamicParams(GetEventsListRequest query)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pageNumber", query.PaginationRequest!.PageNumber);
            parameters.Add("@pageSize", query.PaginationRequest.PageSize);
            parameters.Add("@category", query.Category);
            parameters.Add("@status", query.Status);
            parameters.Add("@startDate", query.StartDate);
            parameters.Add("@endDate", query.EndDate);
            parameters.Add("@location", query.Location);
            return parameters;
        }

        public async Task<(List<EventWithDisplayName> events, int totalCount)> GetEventsAndCountAsync(
             LocationDetail? userLocation,
             double? maxDistanceKm,
             UserProfile? userProfile,
             EventCategory? category,
             EventType? eventType,
             string? createdBy,
             string? eventTitle,
             EventStatus? status,
             bool? includeReviews,
             int? minRating,
             PaginationRequest pagination)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = FilterDynamicParams(
                userLocation,
                maxDistanceKm,
                userProfile,
                category,
                eventType,
                createdBy,
                eventTitle,
                status,
                includeReviews,
                minRating,
                pagination);

            await connection.ExecuteAsync("sp_GetEvents", parameters, commandType: CommandType.StoredProcedure);

            var totalCount = parameters.Get<int>("@p_total_count");
            var jsonEvents = parameters.Get<string>("@p_events");
            var events = string.IsNullOrEmpty(jsonEvents)
                ? new List<EventWithDisplayName>()
                : JsonConvert.DeserializeObject<List<EventWithDisplayName>>(jsonEvents) ?? new List<EventWithDisplayName>();

            return (events, totalCount);
        }

        private static DynamicParameters FilterDynamicParams(
                LocationDetail? userLocation,
                double? maxDistanceKm,
                UserProfile? userProfile,
                EventCategory? category,
                EventType? eventType,
                string? createdBy,
                string? eventTitle,
                EventStatus? status,
                bool? includeReviews,
                int? minRating,
                PaginationRequest pagination)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@p_user_lat", userLocation?.Location?.Lat ?? userLocation?.Results.FirstOrDefault()?.Geometry.Location.Lat ?? (object)DBNull.Value);
            parameters.Add("@p_user_lng", userLocation?.Location?.Lng ?? userLocation?.Results.FirstOrDefault()?.Geometry.Location.Lng ?? (object)DBNull.Value);
            parameters.Add("@p_max_distance_km", maxDistanceKm ?? (object)DBNull.Value);
            parameters.Add("@p_gender", userProfile?.Gender?.GenderType.ToString() ?? (object)DBNull.Value); // Explicitly convert enum to string
            parameters.Add("@p_race", userProfile?.Ethnicity?.EthnicityType ?? (object)DBNull.Value);
            parameters.Add("@p_passions", userProfile?.Passions.Any() == true ? string.Join(",", userProfile.Passions) : (object)DBNull.Value);
            parameters.Add("@p_category", category.HasValue ? (int)category.Value : (object)DBNull.Value);
            parameters.Add("@p_event_type", eventType.HasValue ? (int)eventType.Value : (object)DBNull.Value);
            parameters.Add("@p_created_by", createdBy ?? (object)DBNull.Value);
            parameters.Add("@p_event_title", eventTitle ?? (object)DBNull.Value);
            parameters.Add("@p_status", status.HasValue ? (int)status.Value : (object)DBNull.Value);
            parameters.Add("@p_include_reviews", includeReviews.HasValue ? includeReviews.Value : (object)DBNull.Value);
            parameters.Add("@p_min_rating", minRating ?? (object)DBNull.Value);
            parameters.Add("@p_page_number", pagination.PageNumber);
            parameters.Add("@p_page_size", Math.Min(pagination.PageSize, 5)); // Cap at 5
            parameters.Add("@p_total_count", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@p_events", dbType: DbType.String, direction: ParameterDirection.Output);
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
        
        public async Task<int> CountAllEventsWithPendingApproval()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var count = await connection.QueryFirstOrDefaultAsync<int>("sp_CountAllEventsWithPendingApproval", commandType: CommandType.StoredProcedure);
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