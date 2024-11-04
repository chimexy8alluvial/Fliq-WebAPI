using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatedApprovedListStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Create  PROCEDURE [dbo].[sPGetApproveMatchedList]
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
            
                SELECT MatchInitiatorUserId, Name, matchRequestStatus, DateCreated, PictureUrl, Age 
                FROM MatchRequests
                WHERE UserId = 1 AND matchRequestStatus = 0 
	            ORDER BY DateCreated DESC;
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sPGetApproveMatchedList];");
        }
    }
}
