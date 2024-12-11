using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PromptsCategoriesandQuestionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PromptCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromptQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemGenerated = table.Column<bool>(type: "bit", nullable: false),
                    PromptCategoryId = table.Column<int>(type: "int", nullable: false),
                    CustomPromptId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptQuestions_PromptCategories_PromptCategoryId",
                        column: x => x.PromptCategoryId,
                        principalTable: "PromptCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromptResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserProfileId = table.Column<int>(type: "int", nullable: false),
                    PromptQuestionId = table.Column<int>(type: "int", nullable: false),
                    ResponseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TextResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VoiceNoteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoClipUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromptResponse_PromptQuestions_PromptQuestionId",
                        column: x => x.PromptQuestionId,
                        principalTable: "PromptQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromptResponse_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromptQuestions_PromptCategoryId",
                table: "PromptQuestions",
                column: "PromptCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PromptResponse_PromptQuestionId",
                table: "PromptResponse",
                column: "PromptQuestionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromptResponse_UserProfileId",
                table: "PromptResponse",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromptResponse");

            migrationBuilder.DropTable(
                name: "PromptQuestions");

            migrationBuilder.DropTable(
                name: "PromptCategories");
        }
    }
}
