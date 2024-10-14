using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class pluscodeidthree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationDetails_PlusCode_PlusCodeId",
                table: "LocationDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationResult_PlusCode_PlusCodeId",
                table: "LocationResult");

            migrationBuilder.DropTable(
                name: "PlusCode");

            migrationBuilder.DropIndex(
                name: "IX_LocationResult_PlusCodeId",
                table: "LocationResult");

            migrationBuilder.DropIndex(
                name: "IX_LocationDetails_PlusCodeId",
                table: "LocationDetails");

            migrationBuilder.DropColumn(
                name: "PlusCodeId",
                table: "LocationResult");

            migrationBuilder.DropColumn(
                name: "PlusCodeId",
                table: "LocationDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlusCodeId",
                table: "LocationResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlusCodeId",
                table: "LocationDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PlusCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompoundCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GlobalCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlusCode", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationResult_PlusCodeId",
                table: "LocationResult",
                column: "PlusCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationDetails_PlusCodeId",
                table: "LocationDetails",
                column: "PlusCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationDetails_PlusCode_PlusCodeId",
                table: "LocationDetails",
                column: "PlusCodeId",
                principalTable: "PlusCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationResult_PlusCode_PlusCodeId",
                table: "LocationResult",
                column: "PlusCodeId",
                principalTable: "PlusCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}