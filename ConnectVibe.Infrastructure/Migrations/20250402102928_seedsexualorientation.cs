using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedsexualorientation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Sexualorientation
            migrationBuilder.InsertData(
                table: "SexualOrientation",
                columns: new[] { "Id", "SexualOrientationType","DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Men", DateTime.UtcNow, null, false },
                { 2, "Women",DateTime.UtcNow, null, false },
                { 3, "Both",  DateTime.UtcNow, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all seeded data in reverse order
            migrationBuilder.DeleteData(
                table: "SexualOrientation",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3});
        }
    }
}
