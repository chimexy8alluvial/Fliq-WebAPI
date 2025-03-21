using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpeedDateCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE sp_SpeedDatingCount
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS TotalSpeedDates 
    FROM SpeedDatingEvents;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_SpeedDatingCount;");
        }
    }
}
