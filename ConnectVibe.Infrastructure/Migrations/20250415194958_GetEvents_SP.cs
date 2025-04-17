using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEvents_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE sp_GetEvents
                               @p_user_lat FLOAT,
                    @p_user_lng FLOAT,
                    @p_max_distance_km FLOAT,
                    @p_gender VARCHAR(50),
                    @p_race VARCHAR(100),
                    @p_passions VARCHAR(500),
                    @p_category INT,
                    @p_event_type INT,
                    @p_creator_id INT,
                    @p_page_number INT,
                    @p_page_size INT,
                    @p_total_count INT OUTPUT,
                    @p_events NVARCHAR(MAX) OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @offset INT = (@p_page_number - 1) * @p_page_size;
                    DECLARE @point GEOGRAPHY;
                    IF @p_user_lat IS NOT NULL AND @p_user_lng IS NOT NULL
                        SET @point = GEOGRAPHY::Point(@p_user_lat, @p_user_lng, 4326);

                    -- Count total
                    SELECT @p_total_count = COUNT(*)
                    FROM Events e
                    JOIN Location l ON e.LocationId = l.Id
                    LEFT JOIN EventCriterias ec ON ec.Id = e.EventCriteriaId
                    WHERE e.Status IN (1, 2) -- Upcoming, Ongoing
                    AND (@p_max_distance_km IS NULL OR 
                         GEOGRAPHY::Point(l.Lat, l.Lng, 4326).STDistance(@point) / 1000 <= @p_max_distance_km)
                    AND (@p_gender IS NULL OR ec.Gender = @p_gender)
                    AND (@p_race IS NULL OR ec.Race = @p_race)
                    AND (@p_passions IS NULL OR 
                         (
                             (CHARINDEX('music', LOWER(@p_passions)) > 0 AND ec.EventType = 0) OR
                             (CHARINDEX('movies', LOWER(@p_passions)) > 0 AND ec.EventType = 1) OR
                             (CHARINDEX('comedy', LOWER(@p_passions)) > 0 AND ec.EventType = 2) OR
                             (CHARINDEX('entertainment', LOWER(@p_passions)) > 0 AND ec.EventType = 3)
                         ))
                    AND (@p_category IS NULL OR e.EventCategory = @p_category)
                    AND (@p_event_type IS NULL OR e.EventType = @p_event_type)
                    AND (@p_creator_id IS NULL OR e.UserId = @p_creator_id);

                    -- Fetch paginated events as JSON
                    SET @p_events = (
                        SELECT
                            Id = e.Id,
                            EventTitle = e.EventTitle,
                            EventDescription = e.EventDescription,
                            EventType = CASE e.EventType
                                WHEN 0 THEN 'Physical'
                                WHEN 1 THEN 'Live'
                                ELSE NULL
                            END,
                            EventCategory = CASE e.EventCategory
                                WHEN 0 THEN 'Free'
                                WHEN 1 THEN 'Paid'
                                ELSE NULL
                            END,
                            Status = CASE e.Status
                                WHEN 0 THEN 'PendingApproval'
                                WHEN 1 THEN 'Upcoming'
                                WHEN 2 THEN 'Ongoing'
                                WHEN 3 THEN 'Past'
                                WHEN 4 THEN 'Cancelled'
                                ELSE NULL
                            END,
                            StartDate = e.StartDate,
                            EndDate = e.EndDate,
                            UserId = e.UserId,
                            Location = (
                                SELECT
                                    Lat = l.Lat,
                                    Lng = l.Lng,
                                    IsVisible = CAST(1 AS BIT),
                                    LocationDetail = (
                                        SELECT
                                            Results = (
                                                SELECT
                                                    FormattedAddress = lr.FormattedAddress,
                                                    Geometry = (
                                                        SELECT
                                                            Location = (
                                                                SELECT
                                                                    Lat = l.Lat,
                                                                    Lng = l.Lng
                                                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                                                            ),
                                                            LocationType = 'APPROXIMATE'
                                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                                                    ),
                                                    AddressComponents = JSON_QUERY('[]'),
                                                    Types = JSON_QUERY('[]'),
                                                    PlaceId = ''
                                                FOR JSON PATH
                                            ),
                                            Status = 'OK'
                                        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                                    )
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            ),
                            EventCriteria = (
                                SELECT
                                    EventType = CASE ec.EventType
                                        WHEN 0 THEN 'Music'
                                        WHEN 1 THEN 'Movies'
                                        WHEN 2 THEN 'Comedy'
                                        WHEN 3 THEN 'Entertainment'
                                        ELSE NULL
                                    END,
                                    Gender = ec.Gender,
                                    Race = ec.Race
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                        FROM Events e
                       JOIN Location l ON e.LocationId = l.Id
                       LEFT JOIN EventCriterias ec ON ec.Id = e.EventCriteriaId
	                   LEFT JOIN LocationDetails ld ON l.Id = ld.LocationId
	                   LEFT JOIN LocationResult lr ON ld.Id = lr.LocationDetailId
                        WHERE e.Status IN (1, 2)
                        AND (@p_max_distance_km IS NULL OR 
                             GEOGRAPHY::Point(l.Lat, l.Lng, 4326).STDistance(@point) / 1000 <= @p_max_distance_km)
                        AND (@p_gender IS NULL OR ec.Gender = @p_gender)
                        AND (@p_race IS NULL OR ec.Race = @p_race)
                        AND (@p_passions IS NULL OR 
                             (
                                 (CHARINDEX('music', LOWER(@p_passions)) > 0 AND ec.EventType = 0) OR
                                 (CHARINDEX('movies', LOWER(@p_passions)) > 0 AND ec.EventType = 1) OR
                                 (CHARINDEX('comedy', LOWER(@p_passions)) > 0 AND ec.EventType = 2) OR
                                 (CHARINDEX('entertainment', LOWER(@p_passions)) > 0 AND ec.EventType = 3)
                             ))
                    AND (@p_category IS NULL OR e.EventCategory = @p_category)
                    AND (@p_event_type IS NULL OR e.EventType = @p_event_type)
                    AND (@p_created_by IS NULL OR u.DisplayName LIKE '%' + @p_created_by + '%')
                    ORDER BY e.StartDate
                    OFFSET @offset ROWS FETCH NEXT @p_page_size ROWS ONLY
                    FOR JSON PATH
                );
            END;
            GO
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEvents;");
        }
    }
}
