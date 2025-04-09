using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountAllEventsWithPendingApproval_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CountAllEventsWithPendingApproval
            AS
            BEGIN
                SET NOCOUNT ON;
                 SELECT COUNT(*) AS PendingApproval FROM Events WHERE Status = 0
                    AND (IsDeleted IS NULL OR IsDeleted = 0);
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountAllEventsWithPendingApproval;");
        }
    }
}
