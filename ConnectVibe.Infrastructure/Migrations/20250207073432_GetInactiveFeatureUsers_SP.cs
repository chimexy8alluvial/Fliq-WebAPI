using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetInactiveFeatureUsers_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetInactiveFeatureUsers
                @ThresholdDate DATETIME
            AS
            BEGIN
                SET NOCOUNT ON;
            
                SELECT UserId, Feature, MIN(LastActiveAt) AS LastActiveAt
                FROM UserFeatureActivities
                WHERE LastActiveAt < @ThresholdDate
                  AND IsDeleted = 0 
                GROUP BY UserId, Feature;
            END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetInactiveFeatureUsers;");
        }
    }
}
