using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removedpluscode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationDetails_PlusCode_PlusCodeId",
                table: "LocationDetails");

            migrationBuilder.DropIndex(
                name: "IX_LocationDetails_PlusCodeId",
                table: "LocationDetails");

            migrationBuilder.DropColumn(
                name: "PlusCodeId",
                table: "LocationDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlusCodeId",
                table: "LocationDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
