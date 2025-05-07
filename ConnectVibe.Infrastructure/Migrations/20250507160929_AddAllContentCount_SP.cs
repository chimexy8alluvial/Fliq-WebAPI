using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAllContentCount_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
           CREATE OR ALTER PROCEDURE sp_GetAllContentCount
            AS
            BEGIN

                DECLARE @Results TABLE (
                    ContentType VARCHAR(50),
                    Count INT
                );
            
                INSERT INTO @Results (ContentType, Count)
                SELECT 'SpeedDatingEvent', COUNT(*) FROM SpeedDatingEvents;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'BlindDate', COUNT(*) FROM BlindDates;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'PromptQuestion', COUNT(*) FROM PromptQuestions;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'Game', COUNT(*) FROM Games;
                
                INSERT INTO @Results (ContentType, Count)
                SELECT 'Events', COUNT(*) FROM Events;
            
                SELECT ContentType, Count FROM @Results;
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllContentCount;");
        }
    }
}
