using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedure_GetAllUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetAllUsers
            AS
            BEGIN
                SELECT *
                FROM Users;
            END;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllUsers");
        }
    }
}
