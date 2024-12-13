using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectVibe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetMatchProfileListSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sPGetMatchedList
    @UserId INT,
    @pageNumber INT = 1,
    @pageSize INT,
    @MatchRequestStatus INT = NULL
AS
BEGIN
    DECLARE @from_row INT = 1;
    IF @pageNumber > 1
    BEGIN
        SET @from_row = ((@pageNumber * @pageSize) - (@pageSize)) + 1;
    END;

    ;WITH MatchedList AS (
        SELECT
            MR.MatchInitiatorUserId,
            MR.MatchReceiverUserId,
            CASE
                WHEN MR.MatchInitiatorUserId = @UserId THEN 'Initiated'
                WHEN  MR.MatchReceiverUserId = @UserId THEN 'Received'
            END AS Type,
            U.Name,
            UP.PictureUrl,
            UP.Age,
            MR.MatchRequestStatus,
            MR.DateCreated,
            ROW_NUMBER() OVER (ORDER BY MR.DateCreated DESC) AS RowNum
        FROM MatchRequests MR
        INNER JOIN Users U ON U.Id =
            CASE
                WHEN MR.MatchInitiatorUserId = @UserId THEN MR.MatchReceiverUserId
                WHEN MR.MatchReceiverUserId = @UserId THEN MR.MatchInitiatorUserId
            END
        INNER JOIN UserProfiles UP ON UP.UserId =
            CASE
                WHEN MR.MatchInitiatorUserId = @UserId THEN MR.MatchReceiverUserId
                WHEN MR.MatchReceiverUserId = @UserId THEN MR.MatchInitiatorUserId
            END
        WHERE
            (@MatchRequestStatus IS NULL OR MR.MatchRequestStatus = @MatchRequestStatus)
            AND (@UserId IN (MR.MatchInitiatorUserId, MR.MatchReceiverUserId))
    )
    SELECT
        MatchInitiatorUserId,
        MatchReceiverUserId,
        Type,
        Name,
        PictureUrl,
        Age,
        MatchRequestStatus,
        DateCreated
    FROM MatchedList
    WHERE RowNum BETWEEN @from_row AND (@from_row + @pageSize - 1)
    ORDER BY RowNum;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}