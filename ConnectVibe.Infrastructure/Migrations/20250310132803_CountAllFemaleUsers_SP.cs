using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountAllFemaleUsers_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CountAllFemaleUsers
            AS
            BEGIN
                SET NOCOUNT ON;
               SELECT COUNT(DISTINCT u.Id) AS FemaleUserCount
                FROM Users u
                LEFT JOIN UserProfiles up ON u.Id = up.UserId
                LEFT JOIN Gender g ON up.Id = g.UserProfileId
                WHERE 
                    u.IsDeleted = 0
             AND      g.GenderType = 1 ;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountAllFemaleUsers;");
        }
    }
}
