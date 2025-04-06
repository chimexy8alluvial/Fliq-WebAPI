using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sp_GetAllProfileData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
    CREATE OR ALTER PROCEDURE sp_GetAllProfileData
    AS
    BEGIN
        SET NOCOUNT ON;
        
        -- Occupations
        SELECT *
        FROM Occupation
        WHERE IsDeleted = 0;
        
        -- Religions
        SELECT *
        FROM Religion
         WHERE IsDeleted = 0;
        
        -- Ethnicities
        SELECT *
        FROM Ethnicity
        WHERE IsDeleted = 0;
    

        -- EducationStatuses
        SELECT *
        FROM EducationStatus
        WHERE IsDeleted = 0;
    END;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllProfileData;");
        }
    }
}
