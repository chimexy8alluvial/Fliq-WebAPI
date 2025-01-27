using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class stakeupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StakeId",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StakeId1",
                table: "GameSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stakes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameSessionId = table.Column<int>(type: "int", nullable: false),
                    RequesterId = table.Column<int>(type: "int", nullable: false),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stakes", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Stakes_StakeId1",
                table: "GameSessions");

            migrationBuilder.DropTable(
                name: "Stakes");

            migrationBuilder.DropIndex(
                name: "IX_GameSessions_StakeId1",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "StakeId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "StakeId1",
                table: "GameSessions");
        }
    }
}
