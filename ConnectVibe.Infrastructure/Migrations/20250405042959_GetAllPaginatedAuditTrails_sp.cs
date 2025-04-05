using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllPaginatedAuditTrails_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE sp_GetAllPaginatedAuditTrails
    @Name NVARCHAR(255) = NULL, 
    @Page INT = 1,       
    @PageSize INT = 10        
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    CREATE TABLE #AuditTrailsList
    (
        RowNum INT IDENTITY(1,1),
        Name NVARCHAR(255),
        Email NVARCHAR(255),
        AccessType NVARCHAR(255),
        IPAddress NVARCHAR(255),
        AuditAction NVARCHAR(255)
    );

    INSERT INTO #AuditTrailsList
    (Name, Email, AccessType, IPAddress, AuditAction)
    SELECT
        at.UserFirstName + ' ' + at.UserLastName AS Name,
        at.UserEmail AS Email,
        at.UserRole AS AccessType,
        at.IPAddress AS IPAddress,
        at.AuditAction AS AuditAction
    FROM AuditTrails AS at
    WHERE (@Name IS NULL OR at.UserFirstName + ' ' + at.UserLastName LIKE '%' + @Name + '%');

    DECLARE @TotalCount INT;
    SELECT @TotalCount = COUNT(*) FROM #AuditTrailsList;

    SELECT 
        Name,
        Email,
        AccessType,
        IPAddress,
        AuditAction
    FROM #AuditTrailsList
    WHERE RowNum BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
    ORDER BY RowNum;

    SELECT @TotalCount AS TotalCount;

    DROP TABLE #AuditTrailsList;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllPaginatedAuditTrails");
        }
    }
}
