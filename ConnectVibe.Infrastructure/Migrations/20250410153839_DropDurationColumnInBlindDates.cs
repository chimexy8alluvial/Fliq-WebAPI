using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropDurationColumnInBlindDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop the Language column
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");

            // Step 2: Convert GenderType from nvarchar(max) to int with data mapping
            migrationBuilder.AddColumn<int>(
                name: "TempGenderType",
                table: "Genders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                @"
        UPDATE Genders
        SET TempGenderType = CASE GenderType
            WHEN 'Male' THEN 1
            WHEN 'Female' THEN 2
            ELSE 0 END
        WHERE GenderType IS NOT NULL;
        ");

            migrationBuilder.DropColumn(
                name: "GenderType",
                table: "Genders");

            migrationBuilder.RenameColumn(
                name: "TempGenderType",
                table: "Genders",
                newName: "GenderType");

            // Step 3: Add new columns to GameSessions
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

            // Step 4: Add new column to GameRequests
            migrationBuilder.AddColumn<int>(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Step 5: Alter Duration from int to TimeSpan in BlindDates
            // First, drop the default constraint
            migrationBuilder.Sql(
                @"
        ALTER TABLE BlindDates
        DROP CONSTRAINT [DF__BlindDate__Durat__12FDD1B2];
        ");

            // Add a temporary column to store the converted time values
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TempDuration",
                table: "BlindDates",
                type: "time",
                nullable: false,
                defaultValue: TimeSpan.Zero);

            // Convert existing int values (assumed in seconds) to time and store in temp column
            migrationBuilder.Sql(
                @"
        UPDATE BlindDates 
        SET TempDuration = CAST(DATEADD(SECOND, Duration, '00:00:00') AS time)
        WHERE Duration IS NOT NULL;
        ");

            // Drop the old Duration column
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "BlindDates");

            // Rename the temp column to Duration
            migrationBuilder.RenameColumn(
                name: "TempDuration",
                table: "BlindDates",
                newName: "Duration");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Revert Duration from TimeSpan to int in BlindDates
            // Add a temporary column to store the converted int values
            migrationBuilder.AddColumn<int>(
                name: "TempDuration",
                table: "BlindDates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Convert time back to int (seconds) and store in temp column
            migrationBuilder.Sql(
                @"
        UPDATE BlindDates 
        SET TempDuration = DATEDIFF(SECOND, '00:00:00', Duration)
        WHERE Duration IS NOT NULL;
        ");

            // Drop the old Duration column
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "BlindDates");

            // Rename the temp column to Duration
            migrationBuilder.RenameColumn(
                name: "TempDuration",
                table: "BlindDates",
                newName: "Duration");

            // Add back the default constraint
            migrationBuilder.Sql(
                @"
        ALTER TABLE BlindDates
        ADD CONSTRAINT [DF__BlindDate__Durat__12FDD1B2] DEFAULT (0) FOR Duration;
        ");

            // Step 2: Drop columns from GameSessions
            migrationBuilder.DropColumn(
                name: "DisconnectionResolutionOption",
                table: "GameSessions");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "GameSessions");

            // Step 3: Drop column from GameRequests
            migrationBuilder.DropColumn(
                name: "GameDisconnectionResolutionOption",
                table: "GameRequests");

            // Step 4: Revert GenderType from int to nvarchar(max) with data mapping
            migrationBuilder.AddColumn<string>(
                name: "TempGenderType",
                table: "Genders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                @"
        UPDATE Genders
        SET TempGenderType = CASE GenderType
            WHEN 1 THEN 'Male'
            WHEN 2 THEN 'Female'
            ELSE 'Unknown' END
        WHERE GenderType IS NOT NULL;
        ");

            migrationBuilder.DropColumn(
                name: "GenderType",
                table: "Genders");

            migrationBuilder.RenameColumn(
                name: "TempGenderType",
                table: "Genders",
                newName: "GenderType");

            // Step 5: Restore Language column
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
