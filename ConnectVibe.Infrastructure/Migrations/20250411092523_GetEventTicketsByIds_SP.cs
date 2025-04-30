using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetEventTicketsByIds_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
             CREATE PROCEDURE [dbo].[GetEventTicketsByIds]
                @EventTicketIds NVARCHAR(MAX) -- Comma-separated list of IDs
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Convert comma-separated string to a table of IDs
                WITH EventTicketIds AS (
                    SELECT CAST(value AS INT) AS Id
                    FROM STRING_SPLIT(@EventTicketIds, ',')
                )
                SELECT 
                    -- EventTicket fields
                    et.Id AS Id, -- EventTicket's own Id
                    et.TicketId, 
                    et.UserId, 
                    et.PaymentId, 
                    et.SeatNumber, 
                    et.IsRefunded,
                    et.DateCreated AS DateCreated, -- From Record
                    et.DateModified AS DateModified, -- From Record
                    et.IsDeleted AS IsDeleted, -- From Record
                    -- Ticket fields (prefixed to avoid conflicts)
                    t.Id AS Ticket_Id, 
                    t.TicketName, 
                    t.TicketType, 
                    t.TicketDescription, 
                    t.EventDate, 
                    t.CurrencyId, 
                    t.Amount, 
                    t.MaximumLimit, 
                    t.SoldOut, 
                    t.EventId,
                    t.DateCreated AS Ticket_DateCreated, -- From Record for Ticket
                    t.DateModified AS Ticket_DateModified, -- From Record for Ticket
                    t.IsDeleted AS Ticket_IsDeleted -- From Record for Ticket
                FROM [dbo].[EventTickets] et
                INNER JOIN [dbo].[Tickets] t ON et.TicketId = t.Id
                INNER JOIN EventTicketIds eti ON et.Id = eti.Id;
            END
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetEventTicketsByIds;");
        }
    }
}
