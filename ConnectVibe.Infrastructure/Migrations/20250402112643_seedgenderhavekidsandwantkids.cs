using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedgenderhavekidsandwantkids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Gender
            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Id", "GenderType", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Male", DateTime.UtcNow, null, false },
                { 2, "Female", DateTime.UtcNow, null, false },
                { 3, "Both", DateTime.UtcNow, null, false }
                });
            // Seed HaveKids
            migrationBuilder.InsertData(
                table: "HaveKids",
                columns: new[] { "Id", "HaveKidsType", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Yes", DateTime.UtcNow, null, false },
                { 2, "No", DateTime.UtcNow, null, false },
                { 3, "PreferNotToSay", DateTime.UtcNow, null, false }
                });
            // Seed WantKids
            migrationBuilder.InsertData(
                table: "WantKids",
                columns: new[] { "Id", "WantKidsType", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Yes", DateTime.UtcNow, null, false },
                { 2, "No", DateTime.UtcNow, null, false },
                { 3, "PreferNotToSay", DateTime.UtcNow, null, false }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all seeded data in reverse order
            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3 });
            migrationBuilder.DeleteData(
               table: "HaveKids",
               keyColumn: "Id",
               keyValues: new object[] { 1, 2, 3 });
            migrationBuilder.DeleteData(
               table: "WantKids",
               keyColumn: "Id",
               keyValues: new object[] { 1, 2, 3 });

        }
    }
}
