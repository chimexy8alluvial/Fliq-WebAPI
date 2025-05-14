using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchAcrossEntities_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                       CREATE PROCEDURE sp_SearchAcrossEntities
                @SearchTerm NVARCHAR(100)
            AS
            BEGIN
                SET NOCOUNT ON;
                
                SELECT 
                    u.Id, 
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    pp.PictureUrl AS ProfileImageUrl,
                    'User' AS EntityType
                FROM Users u
                LEFT JOIN UserProfiles up ON u.Id = up.UserId
                LEFT JOIN ProfilePhoto pp ON up.Id = pp.UserProfileId 
                WHERE u.IsDeleted = 0 
                  AND (
                      u.FirstName LIKE '%' + @SearchTerm + '%' OR
                      u.LastName LIKE '%' + @SearchTerm + '%' OR
                      u.Email LIKE '%' + @SearchTerm + '%'
                  )
                ORDER BY u.FirstName, u.LastName
                OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
                
                SELECT 
                    Id,
                    EventTitle,
                    StartDate,
                    'Event' AS EntityType
                FROM Events
                WHERE IsDeleted = 0 
                  AND EventTitle LIKE '%' + @SearchTerm + '%'
                ORDER BY StartDate DESC
                OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
             
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_SearchAcrossEntities;");
        }
    }
}
