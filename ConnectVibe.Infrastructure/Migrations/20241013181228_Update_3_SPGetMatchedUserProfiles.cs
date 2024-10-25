using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_3_SPGetMatchedUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                      CREATE OR ALTER PROCEDURE [dbo].[sPGetMatchedUserProfiles]
                          @userId INT,
              @profileTypes NVARCHAR(MAX),  -- JSON format
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
          
              WITH UserProfileRecords AS
              (
                  SELECT up.*, ROW_NUMBER() OVER (ORDER BY up.DateCreated DESC) AS Row_Num
                  FROM dbo.UserProfiles up
                  WHERE up.UserId != @userId
                  AND EXISTS 
                  (
                      SELECT 1 FROM OPENJSON(@profileTypes) AS jsonProfileTypes
                      WHERE up.ProfileTypes LIKE '%' + jsonProfileTypes.value + '%'
                  )
                  AND (@filterByDating IS NULL OR 
                      (up.ProfileTypes LIKE '%"" + CAST(0 AS NVARCHAR(MAX)) + @""%'))
                  AND (@filterByFriendship IS NULL OR 
                      (up.ProfileTypes LIKE '%"" + CAST(1 AS NVARCHAR(MAX)) + @""%'))
              ),
              RecordCount AS
              (
                  SELECT COUNT(*) AS TotalCount FROM UserProfileRecords
              )
              
              SELECT * FROM UserProfileRecords
              CROSS JOIN RecordCount
              WHERE Row_Num BETWEEN @from_row AND (@from_row + @pageSize - 1)
              ORDER BY DateCreated DESC;
          END
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sPGetMatchedUserProfiles];");

        }
    }
}
