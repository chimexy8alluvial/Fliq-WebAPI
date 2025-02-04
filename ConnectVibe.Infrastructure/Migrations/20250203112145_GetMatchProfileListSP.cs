using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetMatchProfileListSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
           CREATE OR ALTER PROCEDURE sPGetMatchedList
                @UserId INT,
                @pageNumber INT = 1,
                @pageSize INT
            AS
            BEGIN
                DECLARE @from_row INT = 1;
                IF @pageNumber > 1
                BEGIN
                    SET @from_row = ((@pageNumber * @pageSize) - (@pageSize)) + 1;
                END;
            
                SELECT MatchInitiatorUserId, matchRequestStatus, DateCreated
                FROM MatchRequests
                WHERE MatchReceiverUserId = @UserId AND matchRequestStatus != 0 
	            ORDER BY DateCreated DESC;
            END


 ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPGetMatchedList;");
        }
    }
}
