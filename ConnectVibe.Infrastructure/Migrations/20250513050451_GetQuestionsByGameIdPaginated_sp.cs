using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetQuestionsByGameIdPaginated_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE GetQuestionsByGameIdPaginated
    @GameId INT,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate pagination parameters
    SET @PageNumber = CASE WHEN @PageNumber < 1 THEN 1 ELSE @PageNumber END;
    SET @PageSize = CASE WHEN @PageSize < 1 THEN 10 ELSE @PageSize END;

    SELECT
        Id,
        GameId,
        QuestionText,
        Options, -- JSON string
        CorrectAnswer,
        DateCreated,
        DateModified,
        IsDeleted
    FROM GameQuestions
    WHERE GameId = @GameId
        AND IsDeleted = 0
    ORDER BY Id 
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetQuestionsByGameIdPaginated");
        }
    }
}
