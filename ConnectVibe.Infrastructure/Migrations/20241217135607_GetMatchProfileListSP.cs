using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
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

    WITH MatchedRecords AS (
        SELECT
            MR.MatchInitiatorUserId,
            MR.MatchReceiverUserId,
            CASE
                WHEN MR.MatchInitiatorUserId = @UserId THEN 'Initiated'
                WHEN MR.MatchReceiverUserId = @UserId THEN 'Received'
            END AS Type,
            U.DisplayName AS Name,
            P.PictureUrl, -- First PictureUrl
            DATEDIFF(YEAR, UP.DOB, GETDATE()) AS Age,
            MR.MatchRequestStatus,
            MR.DateCreated,
            ROW_NUMBER() OVER (ORDER BY MR.DateCreated DESC) AS Row_Num
        FROM MatchRequests MR
        INNER JOIN Users U ON U.Id =
            CASE
                WHEN MR.MatchInitiatorUserId = @UserId THEN MR.MatchReceiverUserId
                WHEN MR.MatchReceiverUserId = @UserId THEN MR.MatchInitiatorUserId
            END
        INNER JOIN UserProfiles UP ON UP.UserId = U.Id
        LEFT JOIN (
            SELECT
                UserProfileId,
                PictureUrl
            FROM (
                SELECT
                    PP.UserProfileId,
                    PP.PictureUrl,
                    ROW_NUMBER() OVER (PARTITION BY PP.UserProfileId ORDER BY PP.Id ASC) AS RowNum
                FROM ProfilePhoto PP
            ) PhotoWithRowNum
            WHERE PhotoWithRowNum.RowNum = 1
        ) P ON P.UserProfileId = UP.Id
        WHERE
            @UserId IN (MR.MatchInitiatorUserId, MR.MatchReceiverUserId)
            AND (@MatchRequestStatus IS NULL OR MR.MatchRequestStatus = @MatchRequestStatus)
    ),
    RecordCount AS (
        SELECT COUNT(*) AS TotalCount FROM MatchedRecords
    )

    SELECT
        MR.MatchInitiatorUserId,
        MR.MatchReceiverUserId,
        MR.Type,
        MR.Name,
        MR.PictureUrl,
        MR.Age,
        MR.MatchRequestStatus,
        MR.DateCreated,
        RC.TotalCount
    FROM MatchedRecords MR
    CROSS JOIN RecordCount RC
    WHERE Row_Num BETWEEN @from_row AND (@from_row + @pageSize - 1)
    ORDER BY MR.DateCreated DESC;
END;

 ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sPGetMatchedList;");
        }
    }
}