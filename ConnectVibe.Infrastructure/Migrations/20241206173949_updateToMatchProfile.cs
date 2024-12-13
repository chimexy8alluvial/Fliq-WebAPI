using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateToMatchProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchRequests_Users_UserId",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "MatchRequests");

            migrationBuilder.RenameColumn(
                name: "matchRequestStatus",
                table: "MatchRequests",
                newName: "MatchRequestStatus");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MatchRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "MatchReceiverUserId",
                table: "MatchRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchRequests_Users_UserId",
                table: "MatchRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchRequests_Users_UserId",
                table: "MatchRequests");

            migrationBuilder.DropColumn(
                name: "MatchReceiverUserId",
                table: "MatchRequests");

            migrationBuilder.RenameColumn(
                name: "MatchRequestStatus",
                table: "MatchRequests",
                newName: "matchRequestStatus");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "MatchRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "MatchRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MatchRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "MatchRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchRequests_Users_UserId",
                table: "MatchRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
