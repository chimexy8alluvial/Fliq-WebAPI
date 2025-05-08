using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetFlaggedSpeedDateCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_FlaggedSpeedDateCount
                AS
                BEGIN
                
                SELECT COUNT(*)
                FROM SpeedDatingEvents
                WHERE IsDeleted = 0 AND IsFlagged = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_FlaggedSpeedDateCount;");
        }
    }
}
