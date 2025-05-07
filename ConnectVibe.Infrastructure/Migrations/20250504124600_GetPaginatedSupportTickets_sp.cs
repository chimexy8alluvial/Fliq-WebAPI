using ErrorOr;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.HelpAndSupport;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using static Fliq.Domain.Common.Errors.Errors;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetPaginatedSupportTickets_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE GetPaginatedSupportTickets
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @RequestType INT = NULL,
    @RequestStatus INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT 
        Id,
        TicketId,
        Title,
        RequesterId,
        RequesterName,
        RequestType,
        RequestStatus,
        GameSessionId,
        DateCreated,
        DateModified,
        IsDeleted
    FROM SupportTickets
    WHERE 
        IsDeleted = 0
        AND (@RequestType IS NULL OR RequestType = @RequestType)
        AND (@RequestStatus IS NULL OR RequestStatus = @RequestStatus)
    ORDER BY DateCreated DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPaginatedSupportTickets");
        }
    }
}
