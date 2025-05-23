using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetALLFlaggedContentCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                       CREATE OR ALTER PROCEDURE sp_GetAllFlaggedContentCount
            AS
            BEGIN
                DECLARE @Results TABLE (
                    ContentType VARCHAR(50),
                    Count INT
                );
            
            
                INSERT INTO @Results (ContentType, Count)
                SELECT 'SpeedDatingEvent', COUNT(*) FROM SpeedDatingEvents WHERE IsFlagged = 1;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'BlindDate', COUNT(*) FROM BlindDates WHERE IsFlagged = 1;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'PromptQuestion', COUNT(*) FROM PromptQuestions WHERE IsFlagged = 1;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'Game', COUNT(*) FROM Games WHERE IsFlagged = 1;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'Events', COUNT(*) FROM Events WHERE IsFlagged = 1;
            
                SELECT ContentType, Count FROM @Results;
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllFlaggedContentCount;");
        }
    }
}
