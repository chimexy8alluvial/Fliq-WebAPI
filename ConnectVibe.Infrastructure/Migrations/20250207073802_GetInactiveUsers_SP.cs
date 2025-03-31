using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetInactiveUsers_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetInactiveUsers
                @ThresholdDate DATETIME
            AS
            BEGIN
                SET NOCOUNT ON;
            
                SELECT * 
                FROM Users
                WHERE  
                IsDeleted = 0
                LastActiveAt < @ThresholdDate;
            END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetInactiveUsers;");
        }
    }
}
