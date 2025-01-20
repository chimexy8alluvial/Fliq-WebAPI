using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificcationToGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameQuestions_GameSessions_GameSessionId",
                table: "GameQuestions");

            migrationBuilder.DropIndex(
                name: "IX_GameQuestions_GameSessionId",
                table: "GameQuestions");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionIndex",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "CurrentTurnPlayerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "GameQuestions");

            migrationBuilder.DropColumn(
                name: "GameSessionId",
                table: "GameQuestions");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "GameQuestions");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "GameQuestions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionIndex",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentTurnPlayerId",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "GameQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameSessionId",
                table: "GameQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "GameQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Theme",
                table: "GameQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameQuestions_GameSessionId",
                table: "GameQuestions",
                column: "GameSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameQuestions_GameSessions_GameSessionId",
                table: "GameQuestions",
                column: "GameSessionId",
                principalTable: "GameSessions",
                principalColumn: "Id");
        }
    }
}
