using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetSingleUserTotalStakeCount_SP : Migration
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
                FROM Stake s
                INNER JOIN GameSession gs ON s.GameSessionId = gs.Id
                WHERE s.RequesterId = @UserId OR s.RecipientId = @UserId;
            END
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetSingleUserTotalStakeCount;");
        }
    }
}
