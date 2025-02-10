using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserLastActiveAt_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_UpdateUserLastActiveAt
                @UserId INT,
                @LastActiveAt DATETIME
            AS
            BEGIN
                SET NOCOUNT ON;
                
                UPDATE Users
                SET LastActiveAt = @LastActiveAt
                WHERE Id = @UserId;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateUserLastActiveAt;");
        }
    }
}
