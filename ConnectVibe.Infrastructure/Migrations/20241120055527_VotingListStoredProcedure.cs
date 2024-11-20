using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VotingListStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Create  PROCEDURE spGetPollDetailsAndPictures
                @UserId INT

            AS
            BEGIN
                SET NOCOUNT ON;

                -- Select all items from VotePollsTbl where UserId matches
                SELECT * 
                FROM VotePolls
                WHERE UserId = @UserId;

                -- Select the first three pictures from ProfilePhotoTbl B where UserId matches
                SELECT TOP 3 B.PictureUrl
                FROM ProfilePhoto B
                WHERE B.UserProfileId = @UserId
                ORDER BY B.Id; -- Assuming PictureId determines the order of pictures
            END
           ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS spGetPollDetailsAndPictures;");
        }
    }
}
