using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetFlaggedPromptQuestionCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CountFlaggedPrompts
            AS
            BEGIN
               
                SELECT COUNT(*) 
                FROM PromptQuestion
                WHERE IsDeleted = 0 AND IsFlagged = 1;
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountFlaggedPrompts;");
        }
    }
}
