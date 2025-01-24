using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPromptResponseRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromptResponse_PromptQuestionId",
                table: "PromptResponse");

            migrationBuilder.AddColumn<int>(
                name: "PromptQuestionId1",
                table: "PromptResponse",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromptResponse_PromptQuestionId_UserProfileId",
                table: "PromptResponse",
                columns: new[] { "PromptQuestionId", "UserProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromptResponse_PromptQuestionId1",
                table: "PromptResponse",
                column: "PromptQuestionId1",
                unique: true,
                filter: "[PromptQuestionId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PromptResponse_PromptQuestions_PromptQuestionId1",
                table: "PromptResponse",
                column: "PromptQuestionId1",
                principalTable: "PromptQuestions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromptResponse_PromptQuestions_PromptQuestionId1",
                table: "PromptResponse");

            migrationBuilder.DropIndex(
                name: "IX_PromptResponse_PromptQuestionId_UserProfileId",
                table: "PromptResponse");

            migrationBuilder.DropIndex(
                name: "IX_PromptResponse_PromptQuestionId1",
                table: "PromptResponse");

            migrationBuilder.DropColumn(
                name: "PromptQuestionId1",
                table: "PromptResponse");

            migrationBuilder.CreateIndex(
                name: "IX_PromptResponse_PromptQuestionId",
                table: "PromptResponse",
                column: "PromptQuestionId",
                unique: true);
        }
    }
}
