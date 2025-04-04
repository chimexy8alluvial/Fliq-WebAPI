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
                    Id, TicketName, TicketType, TicketDescription, EventDate, 
                    CurrencyId, Amount, MaximumLimit, SoldOut, EventId,
                    DateCreated, DateModified, IsDeleted
                FROM [dbo].[Tickets]
                WHERE EventId = @EventId;
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
