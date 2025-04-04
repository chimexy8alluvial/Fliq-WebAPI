using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountAllSponsoredEvents_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CountAllSponsoredEvents
            AS
            BEGIN
                SET NOCOUNT ON;
                 SELECT COUNT(*) AS SponsoredEvents FROM Events 

                WHERE 
                IsDeleted = 0
               AND SponsoredEvent = 1;
            END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountAllSponsoredEvents;");
        }
    }
}
