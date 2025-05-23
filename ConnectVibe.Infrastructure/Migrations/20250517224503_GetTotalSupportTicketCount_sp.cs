using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTotalSupportTicketCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetTotalSupportTicketsCount
    @RequestType NVARCHAR(50) = NULL,
    @RequestStatus NVARCHAR(50) = NULL
AS
BEGIN
    SELECT COUNT(*) AS TotalCount 
    FROM SupportTickets
    WHERE IsDeleted = 0
        AND (@RequestType IS NULL OR RequestType = @RequestType)
        AND (@RequestStatus IS NULL OR RequestStatus = @RequestStatus);
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTotalSupportTicketsCount");
        }
    }
}
