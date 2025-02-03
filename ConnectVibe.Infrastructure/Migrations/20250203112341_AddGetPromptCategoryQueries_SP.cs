using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGetPromptCategoryQueries_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create GetCategoryById stored procedure
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetPromptCategoryById
                @CategoryId INT
            AS
            BEGIN
                SELECT TOP 1 *
                FROM PromptCategories
                WHERE Id = @CategoryId;
            END;
        ");

            // Create GetCategoryByName stored procedure
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetPromptCategoryByName
                @CategoryName NVARCHAR(MAX)
            AS
            BEGIN
                SELECT TOP 1 *
                FROM PromptCategories
                WHERE LOWER(CategoryName) = LOWER(@CategoryName);
            END;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop stored procedures
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetCategoryById;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetPromptCategoryByName;");
        }
    }
}
