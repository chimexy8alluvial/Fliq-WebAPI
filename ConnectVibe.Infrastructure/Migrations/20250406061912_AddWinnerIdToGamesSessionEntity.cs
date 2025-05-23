using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWinnerIdToGamesSessionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop default constraint and column if exists for DisconnectionResolutionOption
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE Name = N'DisconnectionResolutionOption' AND Object_ID = Object_ID(N'GameSessions'))
                BEGIN
                    DECLARE @constraintName NVARCHAR(200)
                    SELECT @constraintName = Name
                    FROM sys.default_constraints
                    WHERE parent_object_id = OBJECT_ID(N'GameSessions')
                      AND parent_column_id = (
                          SELECT column_id FROM sys.columns
                          WHERE Name = N'DisconnectionResolutionOption'
                            AND object_id = OBJECT_ID(N'GameSessions')
                      )
                    IF @constraintName IS NOT NULL
                        EXEC('ALTER TABLE [GameSessions] DROP CONSTRAINT [' + @constraintName + ']')
                    ALTER TABLE [GameSessions] DROP COLUMN [DisconnectionResolutionOption]
                END
            ");

            // Drop default constraint and column if exists for WinnerId
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE Name = N'WinnerId' AND Object_ID = Object_ID(N'GameSessions'))
                BEGIN
                    DECLARE @constraintName NVARCHAR(200)
                    SELECT @constraintName = Name
                    FROM sys.default_constraints
                    WHERE parent_object_id = OBJECT_ID(N'GameSessions')
                      AND parent_column_id = (
                          SELECT column_id FROM sys.columns
                          WHERE Name = N'WinnerId'
                            AND object_id = OBJECT_ID(N'GameSessions')
                      )
                    IF @constraintName IS NOT NULL
                        EXEC('ALTER TABLE [GameSessions] DROP CONSTRAINT [' + @constraintName + ']')
                    ALTER TABLE [GameSessions] DROP COLUMN [WinnerId]
                END
            ");

            // Drop default constraint and column if exists for GameDisconnectionResolutionOption
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns 
                           WHERE Name = N'GameDisconnectionResolutionOption' AND Object_ID = Object_ID(N'GameRequests'))
                BEGIN
                    DECLARE @constraintName NVARCHAR(200)
                    SELECT @constraintName = Name
                    FROM sys.default_constraints
                    WHERE parent_object_id = OBJECT_ID(N'GameRequests')
                      AND parent_column_id = (
                          SELECT column_id FROM sys.columns
                          WHERE Name = N'GameDisconnectionResolutionOption'
                            AND object_id = OBJECT_ID(N'GameRequests')
                      )
                    IF @constraintName IS NOT NULL
                        EXEC('ALTER TABLE [GameRequests] DROP CONSTRAINT [' + @constraintName + ']')
                    ALTER TABLE [GameRequests] DROP COLUMN [GameDisconnectionResolutionOption]
                END
            ");

            migrationBuilder.AddColumn<int>(
                name: "DisconnectionResolutionOption",
                table: "GameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "GameSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisconnectionResolutionOption",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests");
        }
    }
}
