using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatchRequestTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Alter PROCEDURE [dbo].[SpUpdateMatchRequestTbl]
                  @UserId int,
                  @MatchInitiatorUserId int,
                  @matchRequestStatus int,
                  @Name nvarchar(MAX)
      
                AS
                BEGIN
      
                UPDATE [dbo].[MatchRequests]
                SET Name = @Name,
                matchRequestStatus = @matchRequestStatus
                WHERE UserId = @UserId AND MatchInitiatorUserId = @MatchInitiatorUserId

            END            

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[SpUpdateMatchRequestTbl];");
        }
    }
}
