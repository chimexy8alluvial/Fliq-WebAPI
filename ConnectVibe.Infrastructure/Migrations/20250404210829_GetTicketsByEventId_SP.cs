using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTicketsByEventId_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[GetTicketsByEventId]
                @EventId INT
            AS
            BEGIN
                SET NOCOUNT ON;

               SELECT 
                   *
                FROM [dbo].[Tickets] t
	             INNER JOIN [dbo].[Events] e ON e.Id = t.EventId
                WHERE EventId = @EventId
	             AND e.IsDeleted = 0;
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetTicketsByEventId;");
        }
    }
}
