using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBlindDatingEvents_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@";

CREATE TYPE dbo.BlindDatesIdTableType AS TABLE
(
    Id INT
)
");

            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_DeleteBlindDatingEvents
    @BlindDateIds dbo.BlindDatesIdTableType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE BlindDates
    SET IsDeleted = 1
    WHERE Id IN (SELECT Id FROM @BlindDateIds)
    SELECT @@ROWCOUNT AS DeletedCount
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_DeleteBlindDatingEvents;");
        }
    }
}