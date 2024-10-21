using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_4_SPGetMachedUserProfiles : Migration
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
                DECLARE @from_row INT = 1;
                IF @pageNumber > 1
                BEGIN
                    SET @from_row = ((@pageNumber * @pageSize) - (@pageSize)) + 1;
                END;
            
                WITH UserProfileRecords AS
                (
                    SELECT 
                        up.UserId, 
                        up.DOB, 
                        g.Id AS GenderId, 
                        g.GenderType, 
                        g.IsVisible AS GenderVisible,
            			oc.Id AS OccupationId,
            			oc.OccupationName AS OccupationName,
            			oc.IsVisible AS OccupationVisible,
            			ed.Id AS EducationStatusId,
            			ed.EducationLevel AS EducationLevel,
            			ed.IsVisible AS EducationVisible,
                        so.Id AS SexualOrientationId, 
                        so.SexualOrientationType, 
                        so.IsVisible AS SexualOrientationVisible, 
                        r.Id AS ReligionId, 
                        r.ReligionType, 
                        r.IsVisible AS ReligionVisible, 
                        e.Id AS EthnicityId, 
                        e.EthnicityType, 
                        e.IsVisible AS EthnicityVisible, 
                        hk.Id AS HaveKidsId, 
                        hk.HaveKidsType, 
                        hk.IsVisible AS HaveKidsVisible, 
                        wk.Id AS WantKidsId, 
                        wk.WantKidsType, 
                        wk.IsVisible AS WantKidsVisible, 
                        loc.Id AS LocationId, 
                        loc.Lat, 
                        loc.Lng, 
                        loc.IsVisible AS LocationVisible, 
                        up.AllowNotifications,
                        up.Passions,
            			up.ProfileTypes,
                        up.DateCreated,
            			up.DateModified,
            			up.IsDeleted,
            			up.Id,
                        ROW_NUMBER() OVER (ORDER BY up.DateCreated DESC) AS Row_Num
                    FROM dbo.UserProfiles up
                    
                    LEFT JOIN Gender g ON up.GenderId = g.Id
            		LEFT JOIN Occupation oc ON up.OccupationId = oc.Id
            		LEFT JOIN EducationStatus ed ON up.EducationStatusId = ed.Id
                    LEFT JOIN SexualOrientation so ON up.SexualOrientationId = so.Id
                    LEFT JOIN Religion r ON up.ReligionId = r.Id
                    LEFT JOIN Ethnicity e ON up.EthnicityId = e.Id
                    LEFT JOIN HaveKids hk ON up.HaveKidsId = hk.Id
                    LEFT JOIN WantKids wk ON up.WantKidsId = wk.Id
                    LEFT JOIN Location loc ON up.LocationId = loc.Id
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
            
                SELECT * 
                FROM UserProfileRecords
                CROSS JOIN RecordCount
                WHERE Row_Num BETWEEN @from_row AND (@from_row + @pageSize - 1)
                ORDER BY DateCreated DESC;
            END"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[sPGetMatchedUserProfiles];");
        }
    }
}
