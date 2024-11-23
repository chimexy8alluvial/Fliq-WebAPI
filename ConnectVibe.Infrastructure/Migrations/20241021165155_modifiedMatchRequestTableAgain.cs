using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class modifiedMatchRequestTableAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchRequests_ProfilePhoto_PhotosId",
                table: "MatchRequests");

            migrationBuilder.DropIndex(
                name: "IX_MatchRequests_PhotosId",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "PhotosId",
                table: "MatchRequests");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "MatchRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "MatchRequests");

            migrationBuilder.AddColumn<int>(
                name: "PhotosId",
                table: "MatchRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MatchRequests_PhotosId",
                table: "MatchRequests",
                column: "PhotosId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchRequests_ProfilePhoto_PhotosId",
                table: "MatchRequests",
                column: "PhotosId",
                principalTable: "ProfilePhoto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
