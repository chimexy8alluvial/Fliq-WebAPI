using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedprofiledatamig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed EducationStatus
            migrationBuilder.InsertData(
                table: "EducationStatus",
                columns: new[] { "Id", "EducationLevel", "IsVisible", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "High School", true, DateTime.UtcNow, null, false },
                { 2, "Associate Degree", true, DateTime.UtcNow, null, false },
                { 3, "Bachelor's Degree", true, DateTime.UtcNow, null, false },
                { 4, "Master's Degree", true, DateTime.UtcNow, null, false },
                { 5, "Doctorate", true, DateTime.UtcNow, null, false }
                });

            // Seed Ethnicity
            migrationBuilder.InsertData(
                table: "Ethnicity",
                columns: new[] { "Id", "EthnicityType", "IsVisible", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Asian", true, DateTime.UtcNow, null, false },
                { 2, "Black", true, DateTime.UtcNow, null, false },
                { 3, "Hispanic", true, DateTime.UtcNow, null, false },
                { 4, "White", true, DateTime.UtcNow, null, false },
                { 5, "Native American", true, DateTime.UtcNow, null, false },
                { 6, "Pacific Islander", true, DateTime.UtcNow, null, false },
                { 7, "Other", true, DateTime.UtcNow, null, false }
                });

            // Seed Religion
            migrationBuilder.InsertData(
                table: "Religion",
                columns: new[] { "Id", "ReligionType", "IsVisible", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Christianity", true, DateTime.UtcNow, null, false },
                { 2, "Islam", true, DateTime.UtcNow, null, false },
                { 3, "Hinduism", true, DateTime.UtcNow, null, false },
                { 4, "Buddhism", true, DateTime.UtcNow, null, false },
                { 5, "Judaism", true, DateTime.UtcNow, null, false },
                { 6, "Atheism", true, DateTime.UtcNow, null, false },
                { 7, "Other", true, DateTime.UtcNow, null, false }
                });

            // Seed Occupation
            migrationBuilder.InsertData(
                table: "Occupation",
                columns: new[] { "Id", "OccupationName", "IsVisible", "DateCreated", "DateModified", "IsDeleted" },
                values: new object[,]
                {
                { 1, "Student", true, DateTime.UtcNow, null, false },
                { 2, "Software Developer", true, DateTime.UtcNow, null, false },
                { 3, "Doctor", true, DateTime.UtcNow, null, false },
                { 4, "Teacher", true, DateTime.UtcNow, null, false },
                { 5, "Engineer", true, DateTime.UtcNow, null, false },
                { 6, "Artist", true, DateTime.UtcNow, null, false },
                { 7, "Entrepreneur", true, DateTime.UtcNow, null, false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete all seeded data in reverse order
            migrationBuilder.DeleteData(
                table: "Occupation",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            migrationBuilder.DeleteData(
                table: "Religion",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            migrationBuilder.DeleteData(
                table: "Ethnicity",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            migrationBuilder.DeleteData(
                table: "EducationStatus",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
