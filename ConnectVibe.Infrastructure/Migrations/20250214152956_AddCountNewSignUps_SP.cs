using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCountNewSignUps_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            CREATE PROCEDURE sp_CountUsersCreatedInLastDays
                @Days INT
            AS
            BEGIN
                SET NOCOUNT ON;
                SELECT COUNT(*) AS RecentUsers
                FROM Users
                WHERE DateCreated >= DATEADD(DAY, -@Days, GETUTCDATE())
                    AND IsDeleted = 0;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountUsersCreatedInLastDays;");
        }
    }
}
