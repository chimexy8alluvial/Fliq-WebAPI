using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrUpdateUserFeatureActivity_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE sp_AddOrUpdateUserFeatureActivity
                    @Id INT,
                    @UserId INT,
                    @Feature NVARCHAR(255),
                    @LastActiveAt DATETIME,
                	@DateCreated DATETIME,
                	@IsDeleted BIT = 0  -- Provide default value
                AS
                BEGIN
                    SET NOCOUNT ON;
                
                    IF EXISTS (SELECT 1 FROM UserFeatureActivities WHERE Id = @Id)
                    BEGIN
                        UPDATE UserFeatureActivities
                        SET LastActiveAt = @LastActiveAt,
                			IsDeleted = @IsDeleted,
                			DateCreated = @DateCreated
                        WHERE Id = @Id;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO UserFeatureActivities (UserId, Feature, LastActiveAt, IsDeleted, DateCreated)
                        VALUES (@UserId, @Feature, @LastActiveAt, @IsDeleted, @DateCreated);
                    END
                END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_AddOrUpdateUserFeatureActivity;");
        }
    }
}
