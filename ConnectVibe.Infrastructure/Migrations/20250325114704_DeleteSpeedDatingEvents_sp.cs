using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSpeedDatingEvents_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

CREATE TYPE dbo.SpeedDatesIdTableType AS TABLE
(
    Id INT
)
");

            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_DeleteSpeedDatingEvents
    @SpeedDateIds dbo.SpeedDatesIdTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SpeedDatingEvents
    SET IsDeleted = 1
    WHERE Id IN (SELECT Id FROM @SpeedDateIds)

    SELECT @@ROWCOUNT AS DeletedCount
END
");
        }



        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteSpeedDatingEvents;");
        }
    }
}
