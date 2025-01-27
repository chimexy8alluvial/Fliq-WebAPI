using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stakeupdatetwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Stakes_StakeId1",
                table: "GameSessions");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_StakeId1",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "StakeId1",
                table: "GameSessions");

            migrationBuilder.CreateIndex(
                name: "IX_Stakes_GameSessionId",
                table: "Stakes",
                column: "GameSessionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stakes_GameSessions_GameSessionId",
                table: "Stakes",
                column: "GameSessionId",
                principalTable: "GameSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stakes_GameSessions_GameSessionId",
                table: "Stakes");

            migrationBuilder.DropIndex(
                name: "IX_Stakes_GameSessionId",
                table: "Stakes");

            migrationBuilder.AddColumn<int>(
                name: "StakeId1",
                table: "GameSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_StakeId1",
                table: "GameSessions",
                column: "StakeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Stakes_StakeId1",
                table: "GameSessions",
                column: "StakeId1",
                principalTable: "Stakes",
                principalColumn: "Id");
        }
    }
}
