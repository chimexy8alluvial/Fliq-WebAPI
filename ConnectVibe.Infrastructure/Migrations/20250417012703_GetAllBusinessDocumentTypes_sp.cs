﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetAllBusinessDocumentTypes_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
 CREATE OR ALTER PROCEDURE sp_GetAllBusinessDocumentTypes
    AS
    BEGIN
        SELECT *
        FROM BusinessDocumentTypes
        WHERE IsDeleted = 0;
    END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAllBusinessDocumentTypes");
        }
    }
}
