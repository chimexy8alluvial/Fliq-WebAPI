using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PromptResponse_Response : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextResponse",
                table: "PromptResponse");

            migrationBuilder.DropColumn(
                name: "VideoClipUrl",
                table: "PromptResponse");

            migrationBuilder.RenameColumn(
                name: "VoiceNoteUrl",
                table: "PromptResponse",
                newName: "Response");

            migrationBuilder.AddColumn<string>(
                name: "ActionUrl",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButtonText",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

         

            migrationBuilder.RenameColumn(
                name: "SponsoringBudget",
                 table: "SponsoredEventDetails",
                newName: "SponsoringPlan");

            migrationBuilder.AddColumn<DateTime>(
               name: "StartDate",
               table: "SponsoredEventDetails",
               type: "datetime2",
               nullable: false);
            migrationBuilder.AddColumn<DateTime>(
              name: "EndDate",
              table: "SponsoredEventDetails",
              type: "datetime2",
              nullable: false);

            migrationBuilder.DropColumn(
               name: "DurationOfSponsorship",
               table: "SponsoredEventDetails");

            migrationBuilder.DropColumn(
              name: "NumberOfInvitees",
              table: "SponsoredEventDetails");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropColumn(
                name: "ActionUrl",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ButtonText",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "PromptResponse",
                newName: "VoiceNoteUrl");

            migrationBuilder.AddColumn<string>(
                name: "TextResponse",
                table: "PromptResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoClipUrl",
                table: "PromptResponse",
                type: "nvarchar(max)",
                nullable: true);

        }
    }
}
