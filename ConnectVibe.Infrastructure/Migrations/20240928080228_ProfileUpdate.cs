using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "ScreeMode",
                table: "Settings",
                newName: "ScreenMode");

            migrationBuilder.AlterColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SexualOrientationId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "EducationStatusId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OccupationId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileDescription",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileTypes",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "EducationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducationLevel = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Occupation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OccupationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occupation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_EducationStatusId",
                table: "UserProfiles",
                column: "EducationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_OccupationId",
                table: "UserProfiles",
                column: "OccupationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_EducationStatus_EducationStatusId",
                table: "UserProfiles",
                column: "EducationStatusId",
                principalTable: "EducationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Occupation_OccupationId",
                table: "UserProfiles",
                column: "OccupationId",
                principalTable: "Occupation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles",
                column: "SexualOrientationId",
                principalTable: "SexualOrientation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles",
                column: "WantKidsId",
                principalTable: "WantKids",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_EducationStatus_EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Occupation_OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_WantKids_WantKidsId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "EducationStatus");

            migrationBuilder.DropTable(
                name: "Occupation");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "EducationStatusId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "OccupationId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileDescription",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileTypes",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "ScreenMode",
                table: "Settings",
                newName: "ScreeMode");

            migrationBuilder.AlterColumn<int>(
                name: "WantKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SexualOrientationId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HaveKidsId",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_HaveKids_HaveKidsId",
                table: "UserProfiles",
                column: "HaveKidsId",
                principalTable: "HaveKids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_SexualOrientation_SexualOrientationId",
                table: "UserProfiles",
                column: "SexualOrientationId",
                principalTable: "SexualOrientation",
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
    }
}
