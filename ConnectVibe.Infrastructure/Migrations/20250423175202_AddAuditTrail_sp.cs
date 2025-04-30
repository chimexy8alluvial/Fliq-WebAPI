using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fliq.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditTrail_sp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE sp_AddAuditTrail
    @UserId INT,
    @UserFirstName NVARCHAR(100),
    @UserLastName NVARCHAR(100),
    @UserEmail NVARCHAR(255),
    @AuditAction NVARCHAR(100),
    @UserRole NVARCHAR(50),
    @IPAddress NVARCHAR(45),
    @IsDeleted BIT = 0,
    @DateCreated DATETIME2 = NULL,
    @DateModified DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @DateCreated IS NULL
        SET @DateCreated = GETUTCDATE();

    INSERT INTO AuditTrails (
        UserId,
        UserFirstName,
        UserLastName,
        UserEmail,
        AuditAction,
        UserRole,
        IPAddress,
        IsDeleted,
        DateCreated,
        DateModified
    )
    VALUES (
        @UserId,
        @UserFirstName,
        @UserLastName,
        @UserEmail,
        @AuditAction,
        @UserRole,
        @IPAddress,
        @IsDeleted,
        @DateCreated,
        @DateModified
    );

    -- Return the inserted record's ID
    SELECT SCOPE_IDENTITY() AS Id;
END;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS  sp_AddAuditTrail");
        }
    }
}
