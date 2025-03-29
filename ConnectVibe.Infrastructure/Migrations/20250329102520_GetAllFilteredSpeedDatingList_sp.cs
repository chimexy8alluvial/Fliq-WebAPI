
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllFilteredSpeedDatingList_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAllFilteredSpeedDatingList
    @Title NVARCHAR(255) = NULL,
    @Type INT = NULL,
    @Date DATETIME = NULL,
	@Duration TIME = NULL,
    @CreatedBy NVARCHAR(255) = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Date IS NOT NULL 
    BEGIN
        IF @Date < '1753-01-01' 
            SET @Date = '1753-01-01'
        ELSE IF @Date > '9999-12-31'
            SET @Date = '9999-12-31'
    END

    CREATE TABLE #FilteredSpeedDates
    (
        RowNum INT IDENTITY(1,1),
        Title NVARCHAR(255),
        Type INT,
        DateCreated DATETIME,
        Duration TIME,
        SubscriptionType NVARCHAR(50),
        CreatedBy NVARCHAR(255)
    );

    INSERT INTO #FilteredSpeedDates
    (Title, Type, DateCreated, Duration, SubscriptionType, CreatedBy)
    SELECT 
        sd.Title AS Title,
        0 AS Type,
        sd.DateCreated AS DateCreated,
        sd.Duration AS Duration,
        CASE 
            WHEN sub.IsActive = 1 THEN sub.ProductId 
            ELSE NULL 
        END AS SubscriptionType,
        u.DisplayName + '' + u.Email + '' AS CreatedBy
    FROM SpeedDatingEvents sd
    LEFT JOIN Users u ON sd.CreatedByUserId = u.Id
    LEFT JOIN Subscriptions sub ON sub.UserId = u.Id
    WHERE sd.IsDeleted = 0
        AND (@Title IS NULL OR sd.Title LIKE '%' + @Title + '%')
        AND (@Type IS NULL OR 0 = @Type)
        AND (@Date IS NULL OR sd.DateCreated = @Date)
        AND (@CreatedBy IS NULL 
            OR u.DisplayName LIKE '%' + @CreatedBy + '%'
            OR u.Email LIKE '%' + @CreatedBy + '%');

    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM #FilteredSpeedDates;

    SELECT 
        Title,
        Type,
        DateCreated,
        Duration,
        SubscriptionType,
        CreatedBy
    FROM #FilteredSpeedDates
    WHERE RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
    ORDER BY RowNum;

    SELECT @TotalCount AS TotalCount;

    DROP TABLE #FilteredSpeedDates;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllFilteredSpeedDatingList");
        }
    }
}



