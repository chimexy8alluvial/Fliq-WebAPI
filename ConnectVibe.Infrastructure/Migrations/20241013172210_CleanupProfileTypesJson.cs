using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CleanupProfileTypesJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update the ProfileTypes column to remove the outer quotes
            migrationBuilder.Sql(@"
                UPDATE UserProfiles
                SET ProfileTypes = REPLACE(ProfileTypes, '""', '')
                WHERE ProfileTypes LIKE '""%';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
