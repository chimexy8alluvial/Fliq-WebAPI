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
    @CreatedBy NVARCHAR(255) = NULL,
    @SubscriptionType NVARCHAR(50) = NULL,
    @Duration TIME = NULL,
    @DateCreatedFrom DATETIME = NULL,
    @DateCreatedTo DATETIME = NULL, 
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    CREATE TABLE #FilteredSpeedDates
    (
        RowNum INT IDENTITY(1,1),
        Id INT,
        Title NVARCHAR(255),
        Type INT,
        CreatedBy NVARCHAR(255),
        SubscriptionType NVARCHAR(50),
        Duration TIME,
        DateCreated DATETIME
    );
    
    INSERT INTO #FilteredSpeedDates
    (Id, Title, Type, CreatedBy, SubscriptionType, Duration, DateCreated)
    SELECT 
        sd.Id AS Id,
        sd.Title AS Title,
        0 AS Type,
        u.FirstName + ' ' + u.LastName + CHAR(13) + CHAR(10) + u.Email AS CreatedBy,
        CASE 
            WHEN sub.IsActive = 1 THEN sub.ProductId 
            ELSE NULL 
        END AS SubscriptionType,
        sd.Duration AS Duration,
        sd.DateCreated AS DateCreated
    FROM SpeedDatingEvents sd
    LEFT JOIN Users u ON sd.CreatedByUserId = u.Id
    LEFT JOIN Subscriptions sub ON sub.UserId = u.Id
    WHERE sd.IsDeleted = 0
        AND (@Title IS NULL OR sd.Title LIKE '%' + @Title + '%')
        AND (@Type IS NULL OR 0 = @Type)
        AND (@CreatedBy IS NULL 
            OR u.FirstName LIKE '%' + @CreatedBy + '%'
            OR u.Email LIKE '%' + @CreatedBy + '%')
        AND (@SubscriptionType IS NULL OR sub.ProductId = @SubscriptionType)
        AND (@Duration IS NULL OR sd.Duration = @Duration)
        AND (@DateCreatedFrom IS NULL OR sd.DateCreated >= @DateCreatedFrom)
        AND (@DateCreatedTo IS NULL OR sd.DateCreated <= @DateCreatedTo); 
    
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM #FilteredSpeedDates;
    
    SELECT 
        Id,
        Title,
        Type,
        CreatedBy,
        SubscriptionType,
        Duration,
        DateCreated
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