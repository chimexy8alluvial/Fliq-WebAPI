using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetUserFeatureActivity_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetUserFeatureActivity
                @UserId INT,
                @Feature NVARCHAR(255)
            AS
            BEGIN
                SET NOCOUNT ON;
            
                SELECT TOP 1 * 
                FROM UserFeatureActivities
                WHERE UserId = @UserId AND Feature = @Feature;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetUserFeatureActivity;");
        }
    }
}
