using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetRecentUserFeatureActivity_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_GetUserFeatureActivities
                    @UserId INT,
                    @Limit INT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    IF @Limit > 10
                        SET @Limit = 10;

                    SELECT TOP (@Limit) * 
                    FROM UserFeatureActivities
                    WHERE UserId = @UserId
                    ORDER BY LastActiveAt DESC;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetUserFeatureActivities");
        }
    }
}
