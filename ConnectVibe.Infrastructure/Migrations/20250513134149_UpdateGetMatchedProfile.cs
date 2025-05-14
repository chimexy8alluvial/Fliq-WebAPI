using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGetMatchedProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE [dbo].[sPGetMatchedUserProfiles]
            @userId INT,
            @profileTypes NVARCHAR(MAX),  
            @filterByDating BIT = NULL,
            @filterByFriendship BIT = NULL,
            @pageNumber INT = 1,
            @pageSize INT
        AS
        BEGIN
            SET NOCOUNT ON;

            DECLARE @from_row INT = 1;
            IF @pageNumber > 1
            BEGIN
                SET @from_row = ((@pageNumber * @pageSize) - @pageSize) + 1;
            END;

            WITH UserProfileRecords AS
            (
                SELECT 
                    up.UserId, 
                    up.DOB, 
                    g.Id AS GenderId, 
                    g.GenderType, 
                    oc.Id AS OccupationId,
                    oc.OccupationName,
                    ed.Id AS EducationStatusId,
                    ed.EducationLevel,
                    so.Id AS SexualOrientationId, 
                    so.SexualOrientationType, 
                    r.Id AS ReligionId, 
                    r.ReligionType, 
                    e.Id AS EthnicityId, 
                    e.EthnicityType, 
                    hk.Id AS HaveKidsId, 
                    hk.HaveKidsType, 
                    wk.Id AS WantKidsId, 
                    wk.WantKidsType, 
                    loc.Id AS LocationId, 
                    loc.Lat, 
                    loc.Lng, 
                    up.AllowNotifications,
                    up.Passions,
                    up.ProfileTypes,
                    up.DateCreated,
                    up.DateModified,
                    up.IsDeleted,
                    up.Id,
                    (
                        SELECT 
                            pr.Id,
                            pr.ResponseType,
                            pr.Response,
                            pr.PromptQuestionId
                        FROM dbo.PromptResponse pr
                        WHERE pr.UserProfileId = up.Id
                        FOR JSON PATH
                    ) AS PromptResponses,
                    ROW_NUMBER() OVER (ORDER BY up.DateCreated DESC) AS Row_Num
                FROM dbo.UserProfiles up
                LEFT JOIN dbo.Genders g ON up.GenderId = g.Id
                LEFT JOIN dbo.Occupation oc ON up.OccupationId = oc.Id
                LEFT JOIN dbo.EducationStatus ed ON up.EducationStatusId = ed.Id
                LEFT JOIN dbo.SexualOrientation so ON up.SexualOrientationId = so.Id
                LEFT JOIN dbo.Religion r ON up.ReligionId = r.Id
                LEFT JOIN dbo.Ethnicity e ON up.EthnicityId = e.Id
                LEFT JOIN dbo.HaveKids hk ON up.HaveKidsId = hk.Id
                LEFT JOIN dbo.WantKids wk ON up.WantKidsId = wk.Id
                LEFT JOIN dbo.Location loc ON up.LocationId = loc.Id
                WHERE up.UserId != @userId
                AND EXISTS 
                (
                    SELECT 1 
                    FROM OPENJSON(@profileTypes) AS jsonProfileTypes
                    WHERE up.ProfileTypes LIKE '%' + jsonProfileTypes.value + '%'
                )
                AND (@filterByDating IS NULL OR 
                     (up.ProfileTypes LIKE '%""' + CAST(0 AS NVARCHAR(MAX)) + '""%'))
                AND (@filterByFriendship IS NULL OR 
                     (up.ProfileTypes LIKE '%""' + CAST(1 AS NVARCHAR(MAX)) + '""%'))
            ),
            RecordCount AS
            (
                SELECT COUNT(*) AS TotalCount 
                FROM UserProfileRecords
            )
            SELECT * 
            FROM UserProfileRecords
            CROSS JOIN RecordCount
            WHERE Row_Num BETWEEN @from_row AND (@from_row + @pageSize - 1)
            ORDER BY DateCreated DESC;
        END;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [dbo].[sPGetMatchedUserProfiles]");
        }
    }
}
