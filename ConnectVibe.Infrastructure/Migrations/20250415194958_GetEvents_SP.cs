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
                    @p_status INT,
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

                    -- Use a CTE to avoid duplicating WHERE logic
                    WITH FilteredEvents AS (
                        SELECT
                            e.Id,
                            e.EventTitle,
                            e.EventDescription,
                            e.EventType,
                            e.EventCategory,
                            e.Status,
                            e.StartDate,
                            e.EndDate,
                            e.UserId,
                            l.Latitude,
                            l.Longitude,
                            l.FormattedAddress,
                            ec.EventType AS CriteriaEventType,
                            ec.Gender,
                            ec.Race
                        FROM Events e
                        JOIN Locations l ON e.Id = l.EventId
                        LEFT JOIN EventCriteria ec ON e.Id = ec.EventId
                        WHERE (@p_status IS NULL OR e.Status = @p_status)
                        AND (@p_max_distance_km IS NULL OR 
                             GEOGRAPHY::Point(l.Latitude, l.Longitude, 4326).STDistance(@point) / 1000 <= @p_max_distance_km)
                        AND (@p_gender IS NULL OR ec.Gender = @p_gender)
                        AND (@p_race IS NULL OR ec.Race = @p_race)
                        AND (@p_passions IS NULL OR 
                             EXISTS (
                                 SELECT 1
                                 FROM (VALUES 
                                     ('music', 0),
                                     ('movies', 1),
                                     ('comedy', 2),
                                     ('entertainment', 3)
                                 ) AS PassionMap(Passion, EventType)
                                 WHERE CHARINDEX(Passion, LOWER(@p_passions)) > 0
                                 AND ec.EventType = PassionMap.EventType
                             ))
                        AND (@p_category IS NULL OR e.EventCategory = @p_category)
                        AND (@p_event_type IS NULL OR e.EventType = @p_event_type)
                        AND (@p_creator_id IS NULL OR e.UserId = @p_creator_id)
                    )

                    -- Get total count
                    SELECT @p_total_count = COUNT(*)
                    FROM FilteredEvents;

                    -- Fetch paginated events as JSON
                    SET @p_events = (
                        SELECT
                            Id,
                            EventTitle,
                            EventDescription,
                            EventType = CASE EventType
                                WHEN 0 THEN 'Physical'
                                WHEN 1 THEN 'Live'
                                ELSE NULL
                            END,
                            EventCategory = CASE EventCategory
                                WHEN 0 THEN 'Free'
                                WHEN 1 THEN 'Paid'
                                ELSE NULL
                            END,
                            Status = CASE Status
                                WHEN 0 THEN 'PendingApproval'
                                WHEN 1 THEN 'Upcoming'
                                WHEN 2 THEN 'Ongoing'
                                WHEN 3 THEN 'Past'
                                WHEN 4 THEN 'Cancelled'
                                ELSE NULL
                            END,
                            StartDate,
                            EndDate,
                            UserId,
                            Location = (
                                SELECT
                                    Lat = Latitude,
                                    Lng = Longitude,
                                    IsVisible = CAST(1 AS BIT),
                                    LocationDetail = (
                                        SELECT
                                            Results = (
                                                SELECT
                                                    FormattedAddress,
                                                    Geometry = (
                                                        SELECT
                                                            Location = (
                                                                SELECT
                                                                    Lat = Latitude,
                                                                    Lng = Longitude
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
                                    EventType = CASE CriteriaEventType
                                        WHEN 0 THEN 'Music'
                                        WHEN 1 THEN 'Movies'
                                        WHEN 2 THEN 'Comedy'
                                        WHEN 3 THEN 'Entertainment'
                                        ELSE NULL
                                    END,
                                    Gender,
                                    Race
                                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                            )
                        FROM FilteredEvents
                        ORDER BY StartDate
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
