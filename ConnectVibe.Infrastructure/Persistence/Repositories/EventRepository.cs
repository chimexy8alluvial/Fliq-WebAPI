using Dapper;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Fliq.Domain.Entities.Profile;
using Microsoft.Data.SqlClient;
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

        public async Task<(IEnumerable<Events> Data, int TotalCount)> GetEventsAsync(
             LocationDetail? userLocation,
             double? maxDistanceKm,
             UserProfile? userProfile,
             EventCategory? category,
             EventType? eventType,
             int? creatorId,
             EventStatus? status,
             PaginationRequest? pagination)
                {
            using var connection = _connectionFactory.CreateConnection() as SqlConnection;
            if (connection == null)
                throw new InvalidOperationException("Connection is not a SqlConnection.");

            var parameters = FilterDynamicParams(userLocation, maxDistanceKm, userProfile, category, eventType, creatorId, status, pagination);

            // Log parameters for debugging
            _logger.LogDebug($"Calling sp_GetEvents with params: lat={parameters.Get<object>("@p_user_lat")}, lng={parameters.Get<object>("@p_user_lng")}, passions={parameters.Get<object>("@p_passions")}, gender={parameters.Get<object>("@p_gender")}, race={parameters.Get<object>("@p_race")}");

            await connection.OpenAsync();

            try
            {
                await connection.ExecuteAsync(
                    "sp_GetEvents",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                int totalCount = parameters.Get<int>("p_total_count");
                string? jsonEvents = parameters.Get<string>("p_events");

                var events = string.IsNullOrEmpty(jsonEvents)
                    ? new List<Events>()
                    : JsonConvert.DeserializeObject<List<Events>>(jsonEvents, new JsonSerializerSettings
                    {
                        Error = (sender, args) => { args.ErrorContext.Handled = true; }
                    }) ?? new List<Events>();

                _logger.LogDebug($"sp_GetEvents returned {events.Count} events, totalCount={totalCount}");

                return (events.AsEnumerable(), totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetEventsAsync: {ex.Message}");
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        private static DynamicParameters FilterDynamicParams(
            LocationDetail? userLocation,
            double? maxDistanceKm,
            UserProfile? userProfile,
            EventCategory? category,
            EventType? eventType,
            int? creatorId,
            EventStatus? status,
            PaginationRequest? pagination)
        {
            var parameters = new DynamicParameters();

            // Safely handle userLocation
            var lat = userLocation?.Location?.Lat
                ?? (userLocation?.Results != null && userLocation.Results.Any() ? userLocation.Results.First().Geometry.Location.Lat : (object)DBNull.Value);
            var lng = userLocation?.Location?.Lng
                ?? (userLocation?.Results != null && userLocation.Results.Any() ? userLocation.Results.First().Geometry.Location.Lng : (object)DBNull.Value);

            // Convert Passions to lowercase to match sp_GetEvents expectations
            var passions = userProfile?.Passions != null && userProfile.Passions.Any()
                ? string.Join(",", userProfile.Passions.Select(p => p.ToLowerInvariant()))
                : (object)DBNull.Value;

            parameters.Add("@p_user_lat", lat);
            parameters.Add("@p_user_lng", lng);
            parameters.Add("@p_max_distance_km", maxDistanceKm ?? (object)DBNull.Value);
            parameters.Add("@p_gender", userProfile?.Gender != null ? userProfile.Gender.GenderType : (object)DBNull.Value);
            parameters.Add("@p_race", userProfile?.Ethnicity != null ? userProfile.Ethnicity.EthnicityType : (object)DBNull.Value);
            parameters.Add("@p_passions", passions);
            parameters.Add("@p_category", category.HasValue ? (int)category.Value : (object)DBNull.Value);
            parameters.Add("@p_event_type", eventType.HasValue ? (int)eventType.Value : (object)DBNull.Value);
            parameters.Add("@p_creator_id", creatorId ?? (object)DBNull.Value);
            parameters.Add("@p_status", status.HasValue ? (int)status.Value : (object)DBNull.Value);
            parameters.Add("@p_page_number", pagination?.PageNumber ?? 1);
            parameters.Add("@p_page_size", pagination?.PageSize ?? 5);
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