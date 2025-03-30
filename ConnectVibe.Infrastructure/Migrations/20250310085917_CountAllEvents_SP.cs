using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountAllEvents_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_CountAllEvents
            AS
            BEGIN
                SET NOCOUNT ON;
                 SELECT COUNT(*) AS TotalEvents FROM Events
                 WHERE
				 IsDeleted IS NULL OR IsDeleted = 0  ;
            END;
            ");
        }
    

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_CountAllEvents;");
        }
    }
}
