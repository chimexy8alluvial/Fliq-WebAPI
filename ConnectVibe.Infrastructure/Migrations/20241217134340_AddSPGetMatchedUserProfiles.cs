using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSPGetMatchedUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE [dbo].[sPGetMatchedUserProfiles]
                @userId INT,
                @profileTypes VARCHAR(100),
                @filterByDating BIT = NULL,
                @filterByFriendship BIT = NULL,
                @pageNumber INT = 1,
                @pageSize INT
            AS
            BEGIN
                DECLARE @from_row INT = 1;
                IF @pageNumber > 1
                BEGIN
                    SET @from_row = ((@pageNumber * @pageSize) - (@pageSize)) + 1;
                END;

                DECLARE @ProfileTypesTable TABLE (ProfileType VARCHAR(100));
                INSERT INTO @ProfileTypesTable SELECT value FROM STRING_SPLIT(@profileTypes, ',');

                WITH UserProfileRecords AS
                (
                    SELECT up.*, ROW_NUMBER() OVER (ORDER BY up.DateCreated DESC) AS Row_Num
                    FROM dbo.UserProfiles up
                    WHERE up.UserId != @userId
                    AND EXISTS
                    (
                        SELECT 1 FROM @ProfileTypesTable pt
                        WHERE up.ProfileTypes LIKE '%' + pt.ProfileType + '%'
                    )
                    AND (@filterByDating IS NULL OR
                        (up.ProfileTypes LIKE '%Dating%' AND @filterByDating = 1))
                    AND (@filterByFriendship IS NULL OR
                        (up.ProfileTypes LIKE '%Friendship%' AND @filterByFriendship = 1))
                ),
                RecordCount AS
                (
                    SELECT COUNT(*) AS TotalCount FROM UserProfileRecords
                )

                SELECT * FROM UserProfileRecords
                CROSS JOIN RecordCount
                WHERE Row_Num BETWEEN @from_row AND (@from_row + @pageSize - 1)
                ORDER BY DateCreated DESC;
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[sPGetMatchedUserProfiles]");
        }
    }
}