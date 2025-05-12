using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetSingleUserTotalStakeCount_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_GetSingleUserTotalStakeCount
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*)
    FROM Stakes
    INNER JOIN GameSessions gs ON Stakes.GameSessionId = gs.Id
    WHERE Stakes.RequesterId = @UserId OR Stakes.RecipientId = @UserId;
END
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetSingleUserTotalStakeCount");
        }
    }
}
