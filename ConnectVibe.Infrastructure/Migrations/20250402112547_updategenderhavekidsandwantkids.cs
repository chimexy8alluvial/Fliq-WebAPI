using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updategenderhavekidsandwantkids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gender_UserProfiles_UserProfileId",
                table: "Gender");

            migrationBuilder.DropForeignKey(
                name: "FK_HaveKids_UserProfiles_UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropForeignKey(
                name: "FK_WantKids_UserProfiles_UserProfileId",
                table: "WantKids");

            migrationBuilder.DropIndex(
                name: "IX_WantKids_UserProfileId",
                table: "WantKids");

            migrationBuilder.DropIndex(
                name: "IX_HaveKids_UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gender",
                table: "Gender");

            migrationBuilder.DropIndex(
                name: "IX_Gender_UserProfileId",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "WantKids");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "WantKids");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "HaveKids");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "HaveKids");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Gender");

            migrationBuilder.RenameTable(
                name: "Gender",
                newName: "Genders");

            migrationBuilder.AlterColumn<string>(
                name: "WantKidsType",
                table: "WantKids",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "GenderId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "HaveKidsType",
                table: "HaveKids",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "GenderType",
                table: "Genders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Genders",
                table: "Genders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Genders_GenderId",
                table: "UserProfiles",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId",
                principalTable: "WantKids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Genders_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_GenderId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Genders",
                table: "Genders");

            migrationBuilder.DropColumn(
                name: "GenderId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "WantKidsId",
                table: "UserProfiles");

            migrationBuilder.RenameTable(
                name: "Genders",
                newName: "Gender");

            migrationBuilder.AlterColumn<int>(
                name: "WantKidsType",
                table: "WantKids",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "WantKids",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "WantKids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HaveKidsType",
                table: "HaveKids",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "HaveKids",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "HaveKids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "GenderType",
                table: "Gender",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "Gender",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserProfileId",
                table: "Gender",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gender",
                table: "Gender",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WantKids_UserProfileId",
                table: "WantKids",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HaveKids_UserProfileId",
                table: "HaveKids",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gender_UserProfileId",
                table: "Gender",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Gender_UserProfiles_UserProfileId",
                table: "Gender",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HaveKids_UserProfiles_UserProfileId",
                table: "HaveKids",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WantKids_UserProfiles_UserProfileId",
                table: "WantKids",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
