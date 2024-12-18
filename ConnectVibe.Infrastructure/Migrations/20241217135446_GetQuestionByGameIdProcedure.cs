using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetQuestionByGameIdProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
               CREATE PROCEDURE GetQuestionsByGameId
    @GameId INT,
    @PageNumber INT,
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Retrieve paginated questions
    SELECT
        Id AS GameQuestionId,
        GameId,
        QuestionText,
        Options, -- Assuming stored as JSON or delimited string
        CorrectAnswer,
        Category,
        Level,
        Theme
    FROM
        GameQuestions
    WHERE
        GameId = @GameId
    ORDER BY
        Id
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Retrieve total count of questions for pagination
    SELECT COUNT(*) AS TotalCount
    FROM GameQuestions
    WHERE GameId = @GameId;
END;

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetQuestionsByGameId;");
        }
    }
}