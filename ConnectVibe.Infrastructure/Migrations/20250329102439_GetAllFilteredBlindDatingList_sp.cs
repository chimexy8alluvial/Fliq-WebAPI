
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllFilteredBlindDatingList_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetAllFilteredBlindDatingList
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
    
    CREATE TABLE #FilteredBlindDates
    (
        RowNum INT IDENTITY(1,1),
        Title NVARCHAR(255),
        Type INT,
		CreatedBy NVARCHAR(255),
		SubscriptionType NVARCHAR(50),
        Duration TIME,
		DateCreated DATETIME,
    );
    INSERT INTO #FilteredBlindDates
    (Title, Type, CreatedBy, SubscriptionType, Duration, DateCreated)
    SELECT 
        bd.Title AS Title,
        0 AS Type,
        bd.DateCreated AS DateCreated,
        bd.Duration AS Duration,
        CASE 
            WHEN sub.IsActive = 1 THEN sub.ProductId 
            ELSE NULL 
        END AS SubscriptionType,
        u.DisplayName + ' ' + u.Email + '' AS CreatedBy
    FROM BlindDates bd
    LEFT JOIN Users u ON bd.CreatedByUserId = u.Id
    LEFT JOIN Subscriptions sub ON sub.UserId = u.Id
    WHERE bd.IsDeleted = 0
        AND (@Title IS NULL OR bd.Title LIKE '%' + @Title + '%')
        AND (@Type IS NULL OR 0 = @Type)
        AND (@Date IS NULL OR CAST(bd.DateCreated AS DATE) = CAST(@Date AS DATE))
        AND (@CreatedBy IS NULL 
            OR u.DisplayName LIKE '%' + @CreatedBy + '%'
            OR u.Email LIKE '%' + @CreatedBy + '%');
    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM #FilteredBlindDates;
    SELECT 
        Title,
        Type,
        DateCreated,
        Duration,
        SubscriptionType,
        CreatedBy
    FROM #FilteredBlindDates
    WHERE RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
    ORDER BY RowNum;
    SELECT @TotalCount AS TotalCount;
    DROP TABLE #FilteredBlindDates;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS sp_GetAllFilteredBlindDatingList");
        }
    }
}